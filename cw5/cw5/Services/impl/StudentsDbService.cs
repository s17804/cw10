using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using cw5.DTO.Response;
using cw5.Exceptions;
using cw5.Models;
using cw5.Settings;
using static System.Int32;

namespace cw5.Services.impl
{
    public class StudentsDbService : IStudentsDbService
    {
        public IEnumerable<StudentWithStudiesResponse> GetAllStudents()
        {
            const string sqlQuery = "SELECT S.FirstName, S.LastName, S.BirthDate, St.Name, E.Semester FROM Student S " +
                                    " LEFT JOIN Enrollment E ON S.IdEnrollment = E.IdEnrollment " +
                                    " LEFT JOIN Studies St ON E.IdStudy = St.IdStudy";

            using var connection =
                new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand
            {
                Connection = connection,
                CommandText = sqlQuery
            };
            connection.Open();
            var dataReader = command.ExecuteReader();
            var students = new List<StudentWithStudiesResponse>();
            while (dataReader.Read())
            {
                var studentWithStudiesResponse = new StudentWithStudiesResponse
                {
                    FirstName = dataReader["FirstName"].ToString(),
                    LastName = dataReader["LastName"].ToString(),
                    BirthDate = DateTime.Parse(dataReader["BirthDate"].ToString()).ToString("yyyy-MM-dd"),
                    StudiesName = dataReader["Name"].ToString(),
                    Semester = Parse(dataReader["Semester"].ToString())
                };
                students.Add(studentWithStudiesResponse);
            }

            return students;
        }

        public StudentWithStudiesResponse GetStudentByIndexNumberSqlInjectionInVulnerable(string indexNumber)
        {
            var sqlQuery = "SELECT S.FirstName, S.LastName, S.BirthDate, St.Name, E.Semester FROM Student S " +
                           "LEFT JOIN Enrollment E ON S.IdEnrollment = E.IdEnrollment " +
                           "LEFT JOIN Studies St ON E.IdStudy = St.IdStudy WHERE S.IndexNumber = @indexNumber";
            using var connection =
                new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand
            {
                Connection = connection,
                CommandText = sqlQuery
            };
            command.Parameters.AddWithValue("indexNumber", indexNumber);

            connection.Open();
            var dataReader = command.ExecuteReader();
            if (dataReader.Read())
            {
                return new StudentWithStudiesResponse
                {
                    FirstName = dataReader["FirstName"].ToString(),
                    LastName = dataReader["LastName"].ToString(),
                    BirthDate = DateTime.Parse(dataReader["BirthDate"]
                        .ToString()).ToString("yyyy-MM-dd"),
                    StudiesName = dataReader["Name"].ToString(),
                    Semester = Parse(dataReader["Semester"].ToString())
                };

            }
        
            throw new ResourceNotFoundException($"Student with indexNumber = {indexNumber} not found");
        }

        public bool CheckIfStudentExists(string index)
        {
            using var connection = new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand {Connection = connection};
            connection.Open();
            command.CommandText = "SELECT 1 FROM Student S WHERE S.IndexNumber = @IndexNumber";
            command.Parameters.AddWithValue("IndexNumber", index);

            return Convert.ToBoolean(Parse(command.ExecuteScalar().ToString()));
        }
    }
}