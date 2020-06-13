﻿using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using cw5.DTO.Request;
using cw5.DTO.Response;
using cw5.Exceptions;
using cw5.Models;
using cw5.Settings;
using cw5.Utils;
using static System.Int32;

namespace cw5.Services.impl
{
    public class EnrollmentsDbService : IEnrollmentsDbService
    {
        
        public EnrollmentStudentResponse GetEnrollmentByStudentIndexSqlInjectionVulnerable(string indexNumber)
        {
            var sqlQuery =
                "SELECT S.IndexNumber, E.Semester, E.StartDate, St.Name FROM Enrollment " +
                "E LEFT JOIN Student S on e.IdEnrollment = S.IdEnrollment " +
                "LEFT JOIN Studies St on E.IdStudy = St.IdStudy " +
                $"WHERE S.IndexNumber = {indexNumber}";

            using var connection = new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand
            {
                Connection = connection,
                CommandText = sqlQuery
            };
            connection.Open();
            var dataReader = command.ExecuteReader();
            if (dataReader.Read())
            {
                return new EnrollmentStudentResponse
                {
                    IndexNumber = dataReader["IndexNumber"].ToString(),
                    Semester = Parse(dataReader["Semester"].ToString()),
                    StartDate = DateTime.Parse(dataReader["StartDate"].ToString()).ToString("yyyy-MM-dd"),
                    StudiesName = dataReader["Name"].ToString()
                };
            }

            throw new ResourceNotFoundException($"Enrollment for Student with indexNumber = {indexNumber} not found");
        }

        public EnrollmentResponse EnrollNewStudent(EnrollmentStudentRequest enrollmentStudentRequest)
        {
            using var connection = new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand {Connection = connection};
            connection.Open();
            var transaction = connection.BeginTransaction();
            command.Transaction = transaction;

            command.CommandText = "SELECT s.IdStudy FROM Studies s WHERE s.Name = @StudiesName";
            command.Parameters.AddWithValue("StudiesName", enrollmentStudentRequest.Studies);
            var dataReader = command.ExecuteReader();
            if (!dataReader.Read())
            {
                throw new ResourceNotFoundException(
                    $"Studies by name {enrollmentStudentRequest.Studies} does not exist in database");
            }

            var idStudy = Parse(dataReader["IdStudy"].ToString());

            dataReader.Close();
            command.Parameters.Clear();
            command.CommandText =
                "SELECT * FROM Enrollment E WHERE E.Semester = 1 AND E.IdStudy = @IdStudy";
            command.Parameters.AddWithValue("IdStudy", idStudy);
            dataReader = command.ExecuteReader();

            var enrollmentResponse = new EnrollmentResponse();
            if (!dataReader.Read())
            {
                dataReader.Close();
                command.Parameters.Clear();

                command.CommandText =
                    @"INSERT INTO Enrollment(IdEnrollment, Semester, StartDate, IdStudy) 
                OUTPUT INSERTED.IdEnrollment, INSERTED.Semester, INSERTED.StartDate, INSERTED.IdStudy 
                VALUES((SELECT MAX(E.IdEnrollment) FROM Enrollment E) + 1, @Semester, @StartDate, @IdStudy);";
                command.Parameters.AddWithValue("Semester", 1);
                command.Parameters.AddWithValue("StartDate", DateTime.Now);
                command.Parameters.AddWithValue("IdStudy", idStudy);

                enrollmentResponse.IdEnrollment = Parse(command.ExecuteScalar().ToString());
                enrollmentResponse.Semester = Parse(command.Parameters["Semester"].Value.ToString());
                enrollmentResponse.IdStudy = Parse(command.Parameters["IdStudy"].Value.ToString());
                enrollmentResponse.StartDate =
                    DateTime.Parse(command.Parameters["StartDate"].Value.ToString()).ToString("yyyy-MM-dd");
            }
            else
            {
                enrollmentResponse.IdEnrollment = Parse(dataReader["IdEnrollment"].ToString());
                enrollmentResponse.Semester = Parse(dataReader["Semester"].ToString());
                enrollmentResponse.IdStudy = Parse(dataReader["IdStudy"].ToString());
                enrollmentResponse.StartDate =
                    DateTime.Parse(dataReader["StartDate"].ToString()).ToString("yyyy-MM-dd");
            }

            dataReader.Close();
            command.Parameters.Clear();
            command.CommandText = "SELECT S.IndexNumber FROM Student S WHERE IndexNumber = @indexNumber";
            command.Parameters.AddWithValue("indexNumber", enrollmentStudentRequest.Index);
            dataReader = command.ExecuteReader();
            if (dataReader.Read())
            {
                throw new BadRequestException("Student Index number not unique");
                ;
            }

            dataReader.Close();
            command.Parameters.Clear();
            var salt = PasswordUtils.GenerateSalt();

            command.CommandText =
                @"INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment, Password, Salt) 
                VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment, @Password, @Salt)";
            command.Parameters.AddWithValue("IndexNumber", enrollmentStudentRequest.Index);
            command.Parameters.AddWithValue("FirstName", enrollmentStudentRequest.FirstName);
            command.Parameters.AddWithValue("LastName", enrollmentStudentRequest.LastName);
            command.Parameters.AddWithValue("BirthDate", enrollmentStudentRequest.BirthDate);
            command.Parameters.AddWithValue("IdEnrollment", enrollmentResponse.IdEnrollment);
            command.Parameters.AddWithValue("Password",
                PasswordUtils.CreateSaltedPasswordHash(enrollmentStudentRequest.Password, salt));
            command.Parameters.AddWithValue("Salt", salt);
            command.ExecuteNonQuery();

            transaction.Commit();
            return enrollmentResponse;
        }

        public EnrollmentResponse PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            using var connection = new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand {Connection = connection};
            connection.Open();

            command.CommandText = @"SELECT COUNT(1) FROM sys.objects WHERE name='PromoteStudents'";

            if (!Convert.ToBoolean(Parse(command.ExecuteScalar().ToString())))
            {
                var fileInfo = new FileInfo("Resources/promote_students_procedure.sql");
                command.CommandText = fileInfo.OpenText().ReadToEnd();
                command.ExecuteNonQuery();
            }

            command.CommandText = "EXEC PromoteStudents @Semester, @Studies";
            command.Parameters.AddWithValue("Semester", promoteStudentsRequest.Semester);
            command.Parameters.AddWithValue("Studies", promoteStudentsRequest.Studies);
            var dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                return new EnrollmentResponse
                {
                    IdEnrollment = Parse(dataReader["IdEnrollment"].ToString()),
                    Semester = Parse(dataReader["Semester"].ToString()),
                    IdStudy = Parse(dataReader["IdStudy"].ToString()),
                    StartDate = DateTime.Parse(dataReader["StartDate"].ToString()).ToString("yyyy-MM-dd")
                };
            }

            throw new ResourceNotFoundException("Not Found");
        }

        public EnrollmentResponse EnrollNewStudentEntity(EnrollmentStudentRequest enrollmentStudentRequest)
        {
            var context = new StudentDbContext();
            var studies = context.Studies
                .FirstOrDefault(stud => enrollmentStudentRequest.Studies.Equals(stud.Name));

            if (studies == null)
            {
                throw new ResourceNotFoundException("Studies with name = " + enrollmentStudentRequest.Studies 
                                                                           + " dont exist");
            }

            var enrollment = context.Enrollment.FirstOrDefault(enroll => 1.Equals(enroll.Semester)
                                                                         && studies.IdStudy.Equals(enroll.IdStudy));
            
            var enrollmentResponse = new EnrollmentResponse();
            
            if (enrollment == null)
            {
                var enrollmentNew = new Enrollment
                {
                    IdEnrollment = context.Enrollment.Select(enroll =>
                        enroll.IdEnrollment).OrderByDescending(i => i).First() + 1,
                    Semester = 1,
                    StartDate = DateTime.Now,
                    IdStudy = studies.IdStudy
                };

                context.Enrollment.Add(enrollmentNew);
                
                enrollmentResponse.IdEnrollment = enrollmentNew.IdEnrollment;
                enrollmentResponse.Semester = enrollmentNew.Semester;
                enrollmentResponse.IdStudy = enrollmentNew.IdStudy;
                enrollmentResponse.StartDate = enrollmentNew.StartDate.ToString("yyyy-MM-dd");
            }
            else
            {
                enrollmentResponse.IdEnrollment = enrollment.IdEnrollment;
                enrollmentResponse.Semester = enrollment.Semester;
                enrollmentResponse.IdStudy = enrollment.IdStudy;
                enrollmentResponse.StartDate = enrollment.StartDate.ToString("yyyy-MM-dd");
            }

            if (context.Student.Any(student =>  enrollmentStudentRequest.Index.Equals(student.IndexNumber)))
            {
                throw new BadRequestException("Student Index number not unique");
                ;
            }

            var salt = PasswordUtils.GenerateSalt();

            context.Student.Add(new Student
            {
                IndexNumber = enrollmentStudentRequest.Index,
                FirstName = enrollmentStudentRequest.FirstName,
                LastName = enrollmentStudentRequest.LastName,
                BirthDate = enrollmentStudentRequest.BirthDate,
                IdEnrollment = enrollmentResponse.IdEnrollment,
                Password =  PasswordUtils.CreateSaltedPasswordHash(enrollmentStudentRequest.Password, salt),
                Salt = salt
            });

            context.SaveChanges();

            return enrollmentResponse;
        }

        public EnrollmentResponse PromoteStudentsEntity(PromoteStudentsRequest promoteStudentsRequest)
        {
            var semester = promoteStudentsRequest.Semester;
            var studiesName = promoteStudentsRequest.Studies;
            var context = new StudentDbContext();

            var studies = context.Studies
                .FirstOrDefault(stud => studiesName.Equals(stud.Name));

            if (studies == null)
            {
                throw new ResourceNotFoundException("Studies with name = " + studiesName + " dont exist");
            }

            var enrollment = context.Enrollment
                .FirstOrDefault(enroll=> semester.Equals(enroll.Semester)
                                              && studies.IdStudy.Equals(enroll.IdStudy));

            if (enrollment == null)
            {
                throw new ResourceNotFoundException("Enrollment with idStudies = " 
                                                    +  studies.IdStudy + " and semester = " + semester 
                                                    + " dont exist");
            }

            var enrollmentNew = context.Enrollment.FirstOrDefault(enrollment => 
                (semester + 1).Equals(enrollment.Semester) 
                && studies.IdStudy.Equals(enrollment.IdStudy));

            if (enrollmentNew == null)
            {
                enrollmentNew = new Enrollment
                {
                    IdEnrollment = context.Enrollment.Select(enroll => enroll.IdEnrollment)
                        .OrderByDescending(i => i).First() + 1,
                    Semester = semester + 1,
                    IdStudy =  studies.IdStudy,
                    StartDate = DateTime.Today
                };

                context.Enrollment.Add(enrollmentNew);
            }
             
            var students = context.Student.Where(student 
                => enrollment.IdEnrollment.Equals(student.IdEnrollment))
                .ToList();

            students.ForEach(student =>
            {
                student.IdEnrollment = enrollmentNew.IdEnrollment;
            });

            context.SaveChanges();
            
            var response =  new EnrollmentResponse
            {
                Semester = enrollmentNew.Semester,
                IdStudy = enrollmentNew.IdStudy,
                IdEnrollment = enrollmentNew.IdEnrollment,
                StartDate = enrollmentNew.StartDate.ToString("yyyy-MM-dd")
            };
            return response;
        }
    }

}