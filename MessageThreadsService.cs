using Sabio.Data;
using Sabio.Web.Domain;
using Sabio.Web.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Web.Services
{


    public class MessageThreadsService : BaseService
    {
        public static int PostMessageThread(MessageThreadPostRequest model)
        {
            int Id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.MessageThreads_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@ThreadTitle", model.ThreadTitle);
                   paramCollection.AddWithValue("@UserId", model.UserId);


                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out Id);
               }
               );


            return Id;
        }




        public static List<MessageThreads> GetAllMsgThreads()
        {
            List<MessageThreads> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.MessageThreads_SelectAll"
               , inputParamMapper: null
               , map: delegate (IDataReader reader, short set) //function from BaseService
               {
                   MessageThreads p = new MessageThreads();
                   int startingIndex = 0; //startingOrdinal

                   p.Id = reader.GetSafeInt32(startingIndex++);
                   p.ThreadTitle = reader.GetSafeString(startingIndex++);
                   p.UserId = reader.GetSafeString(startingIndex++);
                   p.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   p.LastMessageDate = reader.GetSafeDateTime(startingIndex++);
                   p.CommentCount = reader.GetSafeInt32(startingIndex++);
                   p.ViewCount = reader.GetSafeInt32(startingIndex++);

                   if (list == null)
                   {
                       list = new List<MessageThreads>();
                   }

                   list.Add(p);
               }
               );


            return list;

        }



        public static int PostGroupThreadsMapping(GroupThreadsRequest model)
        {
            int Id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.GroupThreads_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@GroupId", model.GroupId);
                   paramCollection.AddWithValue("@ThreadId", model.ThreadId);

                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out Id);
               }
               );

            return Id;

        }




        public static int PostEventThreadsMapping(EventThreadsRequest model)
        {
            int Id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.EventThreads_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@EventId", model.EventId);
                   paramCollection.AddWithValue("@ThreadId", model.ThreadId);

                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out Id);
               }
               );

            return Id;

        }





        public static List<MessageThreads> GetThreadsByGroupId(int GroupId, int pageNumber, int pageSize)
        {
            List<MessageThreads> list = null;

            //get message threads by groupId
            DataProvider.ExecuteCmd(GetConnection, "MessageThreads_JoinSearchByGroupId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@GroupId", GroupId);
                   paramCollection.AddWithValue("@CurrentPage", pageNumber);
                   paramCollection.AddWithValue("@ItemsPerPage", pageSize);

               }
               , map: delegate (IDataReader reader, short set)
               {
                   MessageThreads m = new MessageThreads();
                   int startingIndex = 0; //startingOrdinal

                   m.Id = reader.GetSafeInt32(startingIndex++);
                   m.ThreadTitle = reader.GetSafeString(startingIndex++);
                   m.UserId = reader.GetSafeString(startingIndex++);
                   m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   m.LastMessageDate = reader.GetSafeDateTime(startingIndex++);
                   m.CommentCount = reader.GetSafeInt32(startingIndex++);
                   m.ViewCount = reader.GetSafeInt32(startingIndex++);

                   UserMini u = new UserMini();

                   u.profileName = reader.GetSafeString(startingIndex++);
                   u.profileLastName = reader.GetSafeString(startingIndex++);
                   u.UserId = reader.GetSafeString(startingIndex++);

                   m.UserInfo = u;

                   if (list == null)
                   {
                       list = new List<MessageThreads>();
                   }

                   list.Add(m);
               }
               );

            return list;
        }



        public static MessageThreads getThreadById(int ThreadId)
        {
            MessageThreads m = null;

            DataProvider.ExecuteCmd(GetConnection, "MessageThreads_JoinSelectThreadInfoById"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@ThreadId", ThreadId);

               }
               , map: delegate (IDataReader reader, short set)
               {
                   m = new MessageThreads();
                   int startingIndex = 0; //startingOrdinal

                   m.Id = reader.GetSafeInt32(startingIndex++);
                   m.ThreadTitle = reader.GetSafeString(startingIndex++);
                   m.UserId = reader.GetSafeString(startingIndex++);
                   m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   m.LastMessageDate = reader.GetSafeDateTime(startingIndex++);
                   m.CommentCount = reader.GetSafeInt32(startingIndex++);
                   m.ViewCount = reader.GetSafeInt32(startingIndex++);

                   UserMini u = new UserMini();

                   u.profileName = reader.GetSafeString(startingIndex++);
                   u.profileLastName = reader.GetSafeString(startingIndex++);
                   u.UserId = reader.GetSafeString(startingIndex++);

                   m.UserInfo = u; 

                   Groups g = new Groups();

                   g.Id = reader.GetSafeInt32(startingIndex++);

                   m.GroupInfo = g;
                   


               }
               );
            return m;
        }



        //FOR PAGINATION:
        public static int GetPageThreads(int GroupId)
        {
            int totalItems = 0;

            DataProvider.ExecuteCmd(GetConnection, "GroupThreads_CountThreadsByGroupId"
                          , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                          {
                              paramCollection.AddWithValue("@GroupId", GroupId);

                          }
                           , map: delegate (IDataReader reader, short set)
                           {

                               int startingIndex = 0;

                               totalItems = reader.GetSafeInt32(startingIndex++);

                           });
            return totalItems;
        }




        public static List<MessageThreads> GetThreadsByEventId(int EventId)
        {
            List<MessageThreads> list = null;

            //get message threads by eventId
            DataProvider.ExecuteCmd(GetConnection, "MessageThreads_JoinSearchByEventId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@EventId", EventId);


               }
               , map: delegate (IDataReader reader, short set)
               {
                   MessageThreads m = new MessageThreads();
                   int startingIndex = 0; //startingOrdinal

                   m.Id = reader.GetSafeInt32(startingIndex++);
                   m.ThreadTitle = reader.GetSafeString(startingIndex++);
                   m.UserId = reader.GetSafeString(startingIndex++);
                   m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   m.LastMessageDate = reader.GetSafeDateTime(startingIndex++);


                   UserMini u = new UserMini();

                   u.profileName = reader.GetSafeString(startingIndex++);
                   u.profileLastName = reader.GetSafeString(startingIndex++);
                   u.UserId = reader.GetSafeString(startingIndex++);

                   m.UserInfo = u;

                   if (list == null)
                   {
                       list = new List<MessageThreads>();
                   }

                   list.Add(m);
               }
               );

            return list;
        }



        public static int PostEventMessageThread(MessageThreadPostRequest model)
        {
            int Id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.MessageThreads_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {

                   paramCollection.AddWithValue("@ThreadTitle", model.ThreadTitle);
                   paramCollection.AddWithValue("@UserId", model.UserId);


                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out Id);
               }
               );


            return Id;
        }



    }
}
