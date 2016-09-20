using Sabio.Data;
using Sabio.Web.Domain;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{

    [RoutePrefix("api/messagethreads")]
    public class MessageThreadsApiController : ApiController
    {
        [Authorize]
        [Route, HttpPost]
        public HttpResponseMessage PostMessageThread(MessageThreadPostRequest model)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string UserId = UserService.GetCurrentUserId();

            model.UserId = UserId;
        
            int threadId = MessageThreadsService.PostMessageThread(model);
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = threadId;

            //insert into GroupThreads mapping table
            GroupThreadsRequest mapping = new GroupThreadsRequest(); 
            mapping.GroupId = model.GroupId;
            mapping.ThreadId = threadId;
            MessageThreadsService.PostGroupThreadsMapping(mapping);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        [Route, HttpGet]
        public HttpResponseMessage GetMsgThreads() //no parameters
        {

            List<MessageThreads> threadData = MessageThreadsService.GetAllMsgThreads(); //no parameters

            ItemsResponse<MessageThreads> response = new ItemsResponse<MessageThreads>();

            response.Items = threadData;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }



        //get list of threads by groupId
        [Route("group/{groupId}"), HttpGet]
        public HttpResponseMessage GetThreadsByGroupId(int groupId, int pageNumber = 1, int pageSize = 10)
        {
            //string senderId = UserService.GetCurrentUserId();

            List<MessageThreads> threadInfo = MessageThreadsService.GetThreadsByGroupId(groupId, pageNumber, pageSize);

            //for pagination
            PagedItemsResponse<MessageThreads> response = new PagedItemsResponse<MessageThreads>();

            response.Items = threadInfo;

            response.PageNumber = pageNumber;

            response.PageSize = pageSize;

            response.TotalItems = MessageThreadsService.GetPageThreads(groupId);


            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        //get a single thread
        [Route("{threadId}"), HttpGet]
        public HttpResponseMessage GetThreadById(int threadId)
        {
            //string senderId = UserService.GetCurrentUserId();

            MessageThreads ThreadInfo = MessageThreadsService.getThreadById(threadId);

            ItemResponse<MessageThreads> response = new ItemResponse<MessageThreads>();

            response.Item = ThreadInfo;


            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        //get list of threads by eventId
        [Route("events/details/{eventId}"), HttpGet]
        public HttpResponseMessage GetThreadsByEventId(int eventId)
        {
            //string senderId = UserService.GetCurrentUserId();

            List<MessageThreads> threadInfo = MessageThreadsService.GetThreadsByEventId(eventId);

            ItemsResponse<MessageThreads> response = new ItemsResponse<MessageThreads>();

            response.Items = threadInfo;

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        [Authorize]
        [Route("events/details/{eventId}"), HttpPost]
        public HttpResponseMessage PostEventMessageThread(MessageThreadPostRequest model)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string UserId = UserService.GetCurrentUserId();
            //Guid myId = new Guid(UserId);
            model.UserId = UserId;

            
            int threadId = MessageThreadsService.PostEventMessageThread(model);
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = threadId;

            //insert into EventThreads mapping table
            EventThreadsRequest mapping = new EventThreadsRequest(); //for inserting into mapping table
            mapping.EventId = model.EventId;
            mapping.ThreadId = threadId;
            MessageThreadsService.PostEventThreadsMapping(mapping);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



    }
}
