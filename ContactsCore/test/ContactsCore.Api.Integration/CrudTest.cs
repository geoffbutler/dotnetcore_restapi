using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ContactsCore.Api.Integration
{
    public abstract class CrudTest<TModel> : BaseTest
        where TModel : Model.Models.Base, new()
    {
        protected ApiRoutes Routes { get; }

        protected CrudTest(string routePrefix)
        {
            Routes = new ApiRoutes(routePrefix);
        }

        protected class ApiRoutes
        {
            private readonly string _routePrefix;

            public ApiRoutes(string routePrefix)
            {
                _routePrefix = routePrefix;
            }

            public string Get(int pageNumber, int pageSize) => $"{_routePrefix}/?pageNumber={pageNumber}&pageSize={pageSize}";
            public string GetByUid(Guid uid) => $"{_routePrefix}/{uid}";
            public string Post() => $"{_routePrefix}/";
            public string Put(Guid uid) => $"{_routePrefix}/{uid}";
            public string Delete(Guid uid) => $"{_routePrefix}/{uid}";
        }
        
        protected async Task<TModel> Post(HttpClient client)
        {            
            var model = GetPostModel();
            var json = JsonConvert.SerializeObject(model);

            var response = await client.PostAsync(
                Routes.Post(),
                new StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    ContentType.ApplicationJson
                    )
                );

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNull();

            var responseModel = JsonConvert.DeserializeObject<TModel>(responseContent);
            responseModel.Should().NotBeNull();

            responseModel.Should().BeEquivalentTo(model);
            return responseModel;
        }

        protected static async Task<TInputModel> PutModel<TInputModel>(HttpClient client, TInputModel model, string route)
        {           
            var json = JsonConvert.SerializeObject(model);

            var response = await client.PutAsync(
                route,
                new StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    ContentType.ApplicationJson
                    )
                );

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNull();

            var responseModel = JsonConvert.DeserializeObject<TInputModel>(responseContent);
            responseModel.Should().NotBeNull();

            responseModel.Should().BeEquivalentTo(model);
            return responseModel;
        }

        protected async Task<TModel> Put(HttpClient client, TModel existingModel = null)            
        {
            Guid uid;
            TModel model;
            if (existingModel == null)
            {
                model = GetPostModel();
                uid = model.Uid;
            }
            else
            {
                uid = existingModel.Uid;
                model = existingModel;

            }
            return await PutModel(client, model, Routes.Put(uid));
        }

        protected async Task Delete(HttpClient client, Guid uid)
        {
            var response = await client.DeleteAsync(
                Routes.Delete(uid)
                );

            response.EnsureSuccessStatusCode();
        }


        protected async Task CreateAndReadByUidImpl()            
        {
            using (var client = GetClient())
            {
                // post                
                var createdModel = await Post(client);

                // get
                var response = await client.GetAsync(Routes.GetByUid(createdModel.Uid));
                response.EnsureSuccessStatusCode();

                // verify
                var responseModel = await DeserializeResponse<TModel>(response);
                responseModel.Should().BeEquivalentTo(createdModel);
            }
        }

        protected async Task CreateAndReadImpl()
        {
            using (var client = GetClient())
            {
                // post many
                var createdModel1 = await Post(client);
                var createdModel2 = await Post(client);
                var createdModels = new List<TModel> { createdModel1, createdModel2 };

                // get many
                var response = await client.GetAsync(Routes.Get(1, int.MaxValue));
                response.EnsureSuccessStatusCode();

                // verify many
                var responseModels = await DeserializeResponse<List<TModel>>(response);
                responseModels = responseModels
                    .Where(o => o.Uid == createdModel1.Uid || o.Uid == createdModel2.Uid)
                    .ToList();
                responseModels.Should().BeEquivalentTo(createdModels);
            }
        }

        protected async Task CreateAndUpdateImpl()
        {
            using (var client = GetClient())
            {
                // create by put
                var createdModel = await Put(client);

                var updatedModel = createdModel;
                updatedModel = GetPutModel(updatedModel);

                // update by put
                await Put(client, updatedModel);

                // get
                var getResponse = await client.GetAsync(Routes.GetByUid(createdModel.Uid));
                getResponse.EnsureSuccessStatusCode();

                // verify
                var getResponseModel = await DeserializeResponse<TModel>(getResponse);
                getResponseModel.Should().BeEquivalentTo(updatedModel);
            }
        }

        protected async Task CreateAndDeleteImpl()
        {
            using (var client = GetClient())
            {
                // create
                var createdModel = await Post(client);

                // verify create
                var getResponse = await client.GetAsync(Routes.GetByUid(createdModel.Uid));
                getResponse.EnsureSuccessStatusCode();

                // delete
                await Delete(client, createdModel.Uid);

                // verify delete
                var getResponse2 = await client.GetAsync(Routes.GetByUid(createdModel.Uid));
                getResponse2.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }


        protected abstract TModel GetPostModel();

        protected abstract TModel GetPutModel(TModel model);
    }
}
