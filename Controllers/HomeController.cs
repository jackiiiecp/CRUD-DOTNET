using CRUDJACKIE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CRUDJACKIE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string connectionString = "Server=tcp:servidor7-pinac.database.windows.net,1433;Initial Catalog=servidor7-jackie;Persist Security Info=False;User ID=servidor7-pinac;Password='LEy!84BKXiCz$T9';MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los productos
        /// </summary>
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var data = new List<Productos>();

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT [Id], [Nombre], [Descripcion], [Precio], [Stock] FROM [Productos]", con))
            {
                try
                {
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            data.Add(new Productos
                            {
                                Id = (int)dr["Id"],
                                Nombre = (string)dr["Nombre"],
                                Descripcion = (string)dr["Descripcion"],
                                Precio = (decimal)dr["Precio"],
                                Stock = (int)dr["Stock"],
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener productos.");
                }

                int totalItems = data.Count;

                var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new PagedViewModel<Productos>
                {
                    Items = pagedData,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = totalItems
                };

                return View(model);
            }

        }

        /// <summary>
        /// Obtener Producto
        /// </summary>
        public IActionResult Details(int id)
        {
            var data = new Productos();

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT [Id], [Nombre], [Descripcion], [Precio], [Stock] FROM [Productos] WHERE [Id] = @i", con))
            {
                cmd.Parameters.Add("@i", SqlDbType.Int).Value = id;

                try
                {
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            data.Id = (int)dr["Id"];
                            data.Nombre = (string)dr["Nombre"];
                            data.Descripcion = (string)dr["Descripcion"];
                            data.Precio = (decimal)dr["Precio"];
                            data.Stock = (int)dr["Stock"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener detalle de producto.");
                }
            }

            return PartialView(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Crea Producto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Productos producto)
        {
            if (!ModelState.IsValid)
            {
                return View(producto);
            }

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"INSERT INTO [Productos] ([Nombre], [Descripcion], [Precio], [Stock]) VALUES (@n, @d, @f, @e);", con))
            {
                cmd.Parameters.Add("@n", SqlDbType.NVarChar, 255).Value = producto.Nombre;
                cmd.Parameters.Add("@d", SqlDbType.NVarChar).Value = producto.Descripcion;
                cmd.Parameters.Add("@f", SqlDbType.Decimal).Value = producto.Precio;
                cmd.Parameters.Add("@e", SqlDbType.Int).Value = producto.Stock;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear producto.");
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el producto.");
                    return View(producto);
                }
            }
        }

        /// <summary>
        /// Editar Producto
        /// </summary>
        public IActionResult Edit(int id)
        {
            var data = new Productos();

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT [Id], [Nombre], [Descripcion], [Precio], [Stock] FROM [Productos] WHERE [Id] = @i", con))
            {
                cmd.Parameters.Add("@i", SqlDbType.Int).Value = id;

                try
                {
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            data.Id = (int)dr["Id"];
                            data.Nombre = (string)dr["Nombre"];
                            data.Descripcion = (string)dr["Descripcion"];
                            data.Precio = (decimal)dr["Precio"];
                            data.Stock = (int)dr["Stock"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener producto para editar.");
                }
            }

            return View(data);
        }

        /// <summary>
        /// Editar Producto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Productos productos)
        {
            if (!ModelState.IsValid)
            {
                return View(productos);
            }

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"UPDATE [Productos] SET [Nombre] = @n, [Descripcion] = @e, [Precio] = @d, [Stock] = @f WHERE [Id] = @i;", con))
            {
                cmd.Parameters.Add("@i", SqlDbType.Int).Value = productos.Id;
                cmd.Parameters.Add("@n", SqlDbType.NVarChar, 255).Value = productos.Nombre;
                cmd.Parameters.Add("@e", SqlDbType.NVarChar).Value = productos.Descripcion;
                cmd.Parameters.Add("@d", SqlDbType.Decimal).Value = productos.Precio;
                cmd.Parameters.Add("@f", SqlDbType.Int).Value = productos.Stock;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar producto.");
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el producto.");
                    return View(productos);
                }
            }
        }

        /// <summary>
        /// Elimina Producto
        /// </summary>
        public IActionResult Delete(int id)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("DELETE FROM [Productos] WHERE [Id] = @i", con))
            {
                cmd.Parameters.Add("@i", SqlDbType.Int).Value = id;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar producto.");
                }
            }

            return RedirectToAction("Index");
        }
    }
}
