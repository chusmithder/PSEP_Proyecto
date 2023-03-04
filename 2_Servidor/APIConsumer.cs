using System;
using System.Collections.Generic;
using RestSharp; //dotnet add package RestSharp

namespace servidorsincrono
{
    class APIConsumer
    {
        private const string BASEURL = "https://localhost:44317/api";

        public static TodoItem GetItems(int id)
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest($"/TodoItems/{id}", Method.Get);
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);//NotFound|OK
            return TodoItem.FromJson(response.Content);
        }

        public static List<TodoItem> GetItems()
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest("TodoItems", Method.Get);
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            return TodoItem.ListFromJson(response.Content);
        }

        public static TodoItem PostItem(TodoItem item)
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest("TodoItems", Method.Post);
            //request.AddParameter("data", data);
            request.AddJsonBody(item.ToJson());
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);//NotFound|Created
            return TodoItem.FromJson(response.Content);
        }

        public static void PutItem(int id, TodoItem item)
        {
            var client = new RestClient(BASEURL);
            // var request = new RestRequest("TodoItems", Method.PUT);
            // request.AddParameter("id", id);
            // request.AddParameter("data", data);
            var request = new RestRequest($"/TodoItems/{id}", Method.Put);
            request.AddJsonBody(item.ToJson());
            var response = client.Execute(request);
            //Console.WriteLine(response.StatusCode);//NoContent|BadRequest
        }

        public static void DeleteItem(int id)
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest($"TodoItems/{id}", Method.Delete);
            var response = client.Execute(request);
            //Console.WriteLine(response.StatusCode);//NotFound|NoContent
        }
    }
}
