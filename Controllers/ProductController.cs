
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using InventryDB.Models;

namespace InventryDB.Controllers
{
    public class ProductController : Controller
    {
        // 1. Establish database connection string
        string cs = @"Data Source=(localdb)\MSSQLLocalDB;
                      Initial Catalog=InventryDB;
                      Integrated Security=True";

        // 2. DISPLAY ALL PRODUCTS
        // SQL: SELECT * FROM Products

        [HttpGet]
        public ActionResult Index()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                // Open connection
                con.Open();

                // Create SQL query
                string query = "SELECT * FROM Products";

                // Establish command object
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Execute query and read records
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = Convert.ToInt32(dr["ProductID"]),
                                ProductName = dr["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(dr["Quantity"]),
                                Price = Convert.ToDecimal(dr["Price"])
                            });
                        }
                    }
                }
            }

            // Connection is closed automatically
            return View(products);
        }

        // 3. OPEN CREATE PAGE
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // 4. INSERT PRODUCT
        // SQL: INSERT INTO Products VALUES (...)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    string query = @"INSERT INTO Products
                                     (ProductName, Quantity, Price)
                                     VALUES
                                     (@ProductName, @Quantity, @Price)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@ProductName",
                            SqlDbType.NVarChar, 100).Value =
                            product.ProductName;

                        cmd.Parameters.Add("@Quantity",
                            SqlDbType.Int).Value = product.Quantity;

                        SqlParameter priceParam =
                            cmd.Parameters.Add("@Price", SqlDbType.Decimal);

                        priceParam.Precision = 10;
                        priceParam.Scale = 2;
                        priceParam.Value = product.Price;

                        // Execute INSERT query
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(product);
        }

        // 5. FETCH PRODUCT FOR EDITING
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            Product product = null;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string query =
                    "SELECT * FROM Products WHERE ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@ProductID",
                        SqlDbType.Int).Value = id.Value;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            product = new Product
                            {
                                ProductID = Convert.ToInt32(dr["ProductID"]),
                                ProductName = dr["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(dr["Quantity"]),
                                Price = Convert.ToDecimal(dr["Price"])
                            };
                        }
                    }
                }
            }

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // 6. UPDATE PRODUCT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    string query = @"UPDATE Products
                                     SET ProductName = @ProductName,
                                         Quantity = @Quantity,
                                         Price = @Price
                                     WHERE ProductID = @ProductID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@ProductID",
                            SqlDbType.Int).Value = product.ProductID;

                        cmd.Parameters.Add("@ProductName",
                            SqlDbType.NVarChar, 100).Value =
                            product.ProductName;

                        cmd.Parameters.Add("@Quantity",
                            SqlDbType.Int).Value = product.Quantity;

                        SqlParameter priceParam =
                            cmd.Parameters.Add("@Price", SqlDbType.Decimal);

                        priceParam.Precision = 10;
                        priceParam.Scale = 2;
                        priceParam.Value = product.Price;

                        // Execute UPDATE query
                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                            return HttpNotFound();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(product);
        }

        // 7. DELETE PRODUCT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string query =
                    "DELETE FROM Products WHERE ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@ProductID",
                        SqlDbType.Int).Value = id;

                    // Execute DELETE query
                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                        return HttpNotFound();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
