﻿using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BookStore.Controllers
{

    public class BookStoreController : ApiController
    {
        #region ConnectionString
        ///actually storing password in public repository 
        ///is dangerous, but anyway. Database does not hold 
        ///anything  important :)
        ///
        ///TODO: save conectionString in Web.config file
        const string conStr =
       @"Data Source=superbookstore.database.windows.net;
        Initial Catalog = BookStore;
        Integrated Security = False;
        User ID = emiraslan;
        Password=Orxan12Aslan24;
        Connect Timeout = 15;
        Encrypt=False;
        TrustServerCertificate=True;
        ApplicationIntent=ReadWrite;
        MultiSubnetFailover=False";
        #endregion



        [HttpPost]
        public IHttpActionResult Login(LoginModel model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                string queryString =
                    $@"select id 
                        from Users
                        where username = '{model.Username.ToLower()}' 
                        or email ='{model.Username.ToLower()}' 
                        and password = '{model.Password}'";

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var reader = (int)(cmd.ExecuteScalar() ?? 0);
                    if (reader < 1)
                        return Ok(new
                        {
                            success = false,
                            message = "Login failed. Check your username/password"
                        });
                }

                return Ok(new
                {
                    success = true,
                    message = "Sucessfully logged in"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult Register(RegisterModel model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                int CartID;
                string queryString = $@"insert into Carts 
                                    values('{model.UserName.ToLower()}')
                                    
                                    Select id from Carts 
                                    where name = '{model.UserName.ToLower()}'";
                using (var cmd = new SqlCommand(queryString, con))
                {
                    CartID = (int)(cmd.ExecuteScalar() ?? 0);
                    if (CartID < 1)
                        return Ok(new
                        {
                            success = false,
                            message = "Inserting new Cart failed."
                        });
                }

             
                queryString =
                $@"insert into Users (FirstName , LastName , Username , Password , Email, CartID) 
                        values(
                                '{model.FirstName}',
                                '{model.LastName}',
                                '{model.UserName.ToLower()}',
                                 {model.Password},
                                '{model.Email.ToLower()}',
                                 {CartID}
                                )";

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return Ok(new
                        {
                            success = false,
                            message = "Registration Failed. Try It Again !"
                        });
                }

                return Ok(new
                {
                    success = true,
                    message = "Sucessfully Registered !"
                });
            }
        }
    }
}
