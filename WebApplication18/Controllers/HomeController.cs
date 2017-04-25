using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WebApplication18.Controllers
{

    public class SimpleFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
    }

    public class Db
    {
        private string _connectionString;

        public Db(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<SimpleFile> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Files";
                connection.Open();
                List<SimpleFile> files = new List<SimpleFile>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SimpleFile sf = new SimpleFile
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Name = (string)reader["Name"]
                    };
                    files.Add(sf);
                }
                return files;

            }

        }

        public void Add(SimpleFile sf)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Files (Name, FileName) VALUES " +
                                  "(@name, @fileName)";
                cmd.Parameters.AddWithValue("@name", sf.Name);
                cmd.Parameters.AddWithValue("@fileName", sf.FileName);
                connection.Open();
                cmd.ExecuteNonQuery();
            }


        }
    }

    public class ShowAllViewModel
    {
        public IEnumerable<SimpleFile> Files { get; set; }
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(string name, HttpPostedFileBase image)
        {
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(image.FileName);
            fileName += extension;

            image.SaveAs(Server.MapPath("~/Images/") + fileName);
            Db d = new Db(Properties.Settings.Default.ConStr);
            d.Add(new SimpleFile { Name = name, FileName = fileName });
            return Redirect("/home/showall");
        }

        public ActionResult ShowAll()
        {
            Db d = new Db(Properties.Settings.Default.ConStr);
            ShowAllViewModel vm = new ShowAllViewModel { Files = d.GetAll() };
            return View(vm);
        }
    }
}

//Title
//description
//name optional/nullable
//phone number
//images
//date listed

//on home page, show all listings, but show only the title, one image, and
//the date it was listed. Next to each listing, there should be a show more button

//Details page - all information, including description, title, name (if available), phone number
//ALL images, and date listed

//On the details page, if the person viewing the item is also the one that listed it, they
//should see a Delete button that allows them to delete it

//Add listing page, should show a form where someone can add all the details, and upload up to five
//images
