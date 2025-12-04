using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.Repositories
{
    public class UserSessionRepository
    {
        public int Create(UserSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            string query = @"
                INSERT INTO UserSessions (UserId, SessionId, DeviceInfo, LoginTime, LastActivityTime, IsActive)
                VALUES (@UserId, @SessionId, @DeviceInfo, @LoginTime, @LastActivityTime, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@UserId", session.UserId, SqlDbType.Int),
                AdoHelper.CreateParameter("@SessionId", session.SessionId, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@DeviceInfo", session.DeviceInfo ?? (object)DBNull.Value, SqlDbType.NVarChar, 500),
                AdoHelper.CreateParameter("@LoginTime", session.LoginTime, SqlDbType.DateTime2),
                AdoHelper.CreateParameter("@LastActivityTime", session.LastActivityTime, SqlDbType.DateTime2),
                AdoHelper.CreateParameter("@IsActive", session.IsActive, SqlDbType.Bit)
            };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        public UserSession GetBySessionId(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return null;

            string query = @"
                SELECT Id, UserId, SessionId, DeviceInfo, LoginTime, LastActivityTime, IsActive
                FROM UserSessions WHERE SessionId = @SessionId";

            var parameters = new[] { AdoHelper.CreateParameter("@SessionId", sessionId, SqlDbType.NVarChar, 100) };
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    return MapReader(reader);
                }
            }
            return null;
        }

        public UserSession GetActiveByUserId(int userId)
        {
            string query = @"
                SELECT TOP 1 Id, UserId, SessionId, DeviceInfo, LoginTime, LastActivityTime, IsActive
                FROM UserSessions WHERE UserId = @UserId AND IsActive = 1 ORDER BY LastActivityTime DESC";

            var parameters = new[] { AdoHelper.CreateParameter("@UserId", userId, SqlDbType.Int) };
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    return MapReader(reader);
                }
            }
            return null;
        }

        public bool DeactivateBySessionId(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return false;

            string query = "UPDATE UserSessions SET IsActive = 0 WHERE SessionId = @SessionId";
            var parameters = new[] { AdoHelper.CreateParameter("@SessionId", sessionId, SqlDbType.NVarChar, 100) };
            int rows = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rows > 0;
        }

        public bool UpdateLastActivity(string sessionId, DateTime lastActivity)
        {
            if (string.IsNullOrEmpty(sessionId)) return false;

            string query = "UPDATE UserSessions SET LastActivityTime = @LastActivityTime WHERE SessionId = @SessionId";
            var parameters = new[]
            {
                AdoHelper.CreateParameter("@LastActivityTime", lastActivity, SqlDbType.DateTime2),
                AdoHelper.CreateParameter("@SessionId", sessionId, SqlDbType.NVarChar, 100)
            };
            int rows = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rows > 0;
        }

        private UserSession MapReader(SqlDataReader reader)
        {
            return new UserSession
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                SessionId = reader.GetString(reader.GetOrdinal("SessionId")),
                DeviceInfo = reader.IsDBNull(reader.GetOrdinal("DeviceInfo")) ? null : reader.GetString(reader.GetOrdinal("DeviceInfo")),
                LoginTime = reader.GetDateTime(reader.GetOrdinal("LoginTime")),
                LastActivityTime = reader.GetDateTime(reader.GetOrdinal("LastActivityTime")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}
