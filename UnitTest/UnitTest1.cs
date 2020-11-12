using AddressBook_RestAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        /// <summary>
        /// translates rest request to http requests for processing
        /// </summary>
        RestClient client;

        /// <summary>
        ///setting the base url given by the user
        /// </summary>
        [TestInitialize]
        public void setUp()
        {
            client = new RestClient("http://localhost:3000");
        }

        private IRestResponse GetEmployeeList()
        {
            /// in this /Employees is refering to the employees list 
            /// it will give us "http://localhost:3000/Employees"
            /// method is specified that what action we want to perform
            RestRequest request = new RestRequest("/Employees", Method.GET);
            ///executing the response 
            IRestResponse response = client.Execute(request);
            return response;
        }

        /// <summary>
        /// On Calling Get API Should Return Employee Details
        /// UC22
        /// </summary>
        [TestMethod]
        public void OnCalling_GetAPI_ShouldReturn_EmployeeObject()
        {
            ///getting irest response from the function
            IRestResponse response = GetEmployeeList();
            ///checking the status code or can be checked by status number
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            ///deserialising object response in the from of list
            List<EmployeeDetails> employeeList = JsonConvert.DeserializeObject<List<EmployeeDetails>>(response.Content);
            ///checking the number of contacts in the list
            Assert.AreEqual(2, employeeList.Count);
            foreach (EmployeeDetails employee in employeeList)
            {
                Console.WriteLine("id: " + employee.id + " firstname : " + employee.firstName + "  lastName :" + employee.lastName);
            }
        }

        /// <summary>
        ///  On Adding MultipleEmployees Calling PostAPI Should Return EmployeeObject
        ///  Uc23
        /// </summary>
        [TestMethod]
        public void OnAddingMultipleEmployees_CallingPostAPI_ShouldReturnEmployeeObject()
        {
            List<EmployeeDetails> multipleEmployeeList = new List<EmployeeDetails>();
            multipleEmployeeList.Add(new EmployeeDetails { id = 3, firstName = "apsy", lastName="Singh",address="Street3",city="Mumbai" });
            multipleEmployeeList.Add(new EmployeeDetails { id = 7, firstName = "gen", lastName = "Farnsa", address = "Street4", city = "Bangalore" });
            multipleEmployeeList.ForEach(employeedata =>
            {
                ///creating request for post method
                RestRequest request = new RestRequest("/Employees", Method.POST);
                ///new json object for adding employees
                JsonObject jsonObject = new JsonObject();
                jsonObject.Add("id", employeedata.id);
                jsonObject.Add("firstname", employeedata.firstName);
                jsonObject.Add("lastName", employeedata.lastName);
                jsonObject.Add("address", employeedata.address);
                jsonObject.Add("city", employeedata.city);
                ///passing application type req , json object ,type of parameters info
                request.AddParameter("application/json", jsonObject, ParameterType.RequestBody);
                ///executing req and storing in response
                IRestResponse response = client.Execute(request);
                ///checking if httpstatus is same
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                ///deserialising response and  checking name,id
                EmployeeDetails employee = JsonConvert.DeserializeObject<EmployeeDetails>(response.Content);
                Assert.AreEqual(employeedata.firstName, employee.firstName);
                Assert.AreEqual(employeedata.id, employee.id);
            });
        }
    }
}
