using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

class Program
{
    private static string connectionStringMin = @"Server=localhost\SQLEXPRESS;Database=comercio_electronico;User id=admin;Password=passwordadmin;TrustServerCertificate=True;";
    private static string connectionString = connectionStringMin; // Asignación inicial
    private static string usuarioLogueado;
    private static bool esAdmin;
    static void Main(string[] args)
    {
        if (Login())
        {
            Console.WriteLine("Login exitoso.");
            MostrarMenu();
        }
        else
        {
            Console.WriteLine("Credenciales incorrectas. Saliendo del programa...");
        }
    }
    static bool Login()
    {
        Console.Write("Nombre de usuario: ");
        string username = Console.ReadLine();

        Console.Write("Contraseña: ");
        string password = Console.ReadLine();

        // Encriptar la contraseña ingresada
        string hashedPassword = EncriptarSHA256(password);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
               
                connection.Open();

                
                string query = "SELECT COUNT(*) FROM Usuario WHERE Nombre = @username AND Contrasena = @password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                  
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", hashedPassword);

                   
                    object result = command.ExecuteScalar();

                    
                    if (result != null && Convert.ToInt32(result) > 0)
                    {
                        usuarioLogueado = username; 
                        return true; 
                    }
                    else
                    {
                        Console.WriteLine("Credenciales incorrectas.");
                        return false; 
                    }
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                return false;
            }
        }
    }
    static void EjecutarComando(string query, Action<SqlCommand> configurarComando)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    configurarComando(command);
                    int filasAfectadas = command.ExecuteNonQuery();
                    Console.WriteLine($"Operación completada. Filas afectadas: {filasAfectadas}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar el comando: {ex.Message}");
            }
        }
    }


    static void MostrarMenu()
    {
        int opcion;

        do
        {
            Console.WriteLine("\nMenú Principal:");
            Console.WriteLine("1. CRUD Usuarios");
            Console.WriteLine("2. CRUD Productos");
            Console.WriteLine("3. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Por favor, ingrese un número válido.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    CrudUsuarios();
                    break;
                case 2:
                    CrudProductos();
                    break;
                case 3:
                    Console.WriteLine("Saliendo...");
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        } while (opcion != 3);
    }
    static void CrudUsuarios()
    {

        int opcion;

        do
        {
            Console.WriteLine("\n--- CRUD Usuarios ---");
            Console.WriteLine("1. Crear Usuario");
            Console.WriteLine("2. Leer Usuarios");
            Console.WriteLine("3. Actualizar Usuario");
            Console.WriteLine("4. Eliminar Usuario");
            Console.WriteLine("5. Regresar al Menú Principal");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Por favor, ingrese un número válido.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    CrearUsuario();
                    break;
                case 2:
                    LeerUsuarios();
                    break;
                case 3:
                    ActualizarUsuario();
                    break;
                case 4:
                    EliminarUsuario();
                    break;
                case 5:
                    Console.WriteLine("Regresando al Menú Principal...");
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        } while (opcion != 5);
    }

    static void CrudProductos()
    {
        int opcion;

        do
        {
            Console.WriteLine("\n--- CRUD Productos ---");
            Console.WriteLine("1. Crear Producto");
            Console.WriteLine("2. Leer Productos");
            Console.WriteLine("3. Actualizar Producto");
            Console.WriteLine("4. Eliminar Producto");
            Console.WriteLine("5. Regresar al Menú Principal");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Por favor, ingrese un número válido.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    CrearProducto();
                    break;
                case 2:
                    LeerProductos();
                    break;
                case 3:
                    ActualizarProducto();
                    break;
                case 4:
                    EliminarProducto();
                    break;
                case 5:
                    Console.WriteLine("Regresando al Menú Principal...");
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        } while (opcion != 5);
    }

    static void CrearUsuario()
    {
        Console.Write("Ingrese el nombre del usuario: ");
        string nombre = Console.ReadLine();
        Console.Write("Ingrese el email del usuario: ");
        string email = Console.ReadLine();
        Console.Write("Ingrese la contraseña del usuario: ");
        string contrasena = Console.ReadLine();

       
        string hashedPassword = EncriptarSHA256(contrasena);

       
        EjecutarComando("INSERT INTO Usuario (nombre, email, contrasena, fecha_registro) VALUES (@nombre, @correo, @password, GETDATE())", cmd =>
        {
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@correo", email);
            cmd.Parameters.AddWithValue("@password", hashedPassword);
        });

        Console.WriteLine("Usuario creado exitosamente.");
    }


    static void LeerUsuarios()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT id, nombre, email, fecha_registro FROM Usuario";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("\nUsuarios Registrados:");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Nombre: {reader["nombre"]}, Email: {reader["email"]}, Fecha de Registro: {reader["fecha_registro"]}");
                }
            }
        }
    }

    static void ActualizarUsuario()
    {
        Console.Write("Ingrese el ID del usuario a actualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Por favor, ingrese un número válido.");
            return;
        }

        Console.Write("Ingrese el nuevo nombre del usuario: ");
        string nuevoNombre = Console.ReadLine();
        Console.Write("Ingrese el nuevo email del usuario: ");
        string nuevoEmail = Console.ReadLine();

        EjecutarComando("UPDATE Usuario SET nombre = @nombre, email = @correo WHERE id = @id", cmd =>
        {
            cmd.Parameters.AddWithValue("@nombre", nuevoNombre);
            cmd.Parameters.AddWithValue("@correo", nuevoEmail);
            cmd.Parameters.AddWithValue("@id", id);
        });

        Console.WriteLine("Usuario actualizado exitosamente.");
    }

    static void EliminarUsuario()
    {
        Console.Write("Ingrese el ID del usuario a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Por favor, ingrese un número válido.");
            return;
        }

        EjecutarComando("DELETE FROM Usuario WHERE id = @id", cmd =>
        {
            cmd.Parameters.AddWithValue("@id", id);
        });

        Console.WriteLine("Usuario eliminado exitosamente.");
    }

    static void CrearProducto()
    {
        Console.Write("Ingrese el nombre del producto: ");
        string nombre = Console.ReadLine();
        Console.Write("Ingrese la descripción del producto: ");
        string descripcion = Console.ReadLine();
        Console.Write("Ingrese el precio del producto: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal precio))
        {
            Console.WriteLine("Por favor, ingrese un precio válido.");
            return;
        }

        Console.Write("Ingrese el ID de la marca: ");
        if (!int.TryParse(Console.ReadLine(), out int marcaId))
        {
            Console.WriteLine("Por favor, ingrese un ID de marca válido.");
            return;
        }

        Console.Write("Ingrese el ID de la categoría: ");
        if (!int.TryParse(Console.ReadLine(), out int categoriaId))
        {
            Console.WriteLine("Por favor, ingrese un ID de categoría válido.");
            return;
        }

       
        EjecutarComando("INSERT INTO Producto (nombre, descripcion, precio, marca_id, categoria_id) VALUES (@nombre, @descripcion, @precio, @marcaId, @categoriaId)", cmd =>
        {
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@descripcion", descripcion);
            cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@marcaId", marcaId);
            cmd.Parameters.AddWithValue("@categoriaId", categoriaId);
        });

        Console.WriteLine("Producto creado exitosamente.");
    }

    static void LeerProductos()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT id, nombre, descripcion, precio, marca_id, categoria_id FROM Producto";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("\nProductos Registrados:");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Nombre: {reader["nombre"]}, Descripción: {reader["descripcion"]}, Precio: {reader["precio"]}, Marca ID: {reader["marca_id"]}, Categoría ID: {reader["categoria_id"]}");
                }
            }
        }
    }

    static void ActualizarProducto()
    {
        Console.Write("Ingrese el ID del producto a actualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Por favor, ingrese un número válido.");
            return;
        }

        Console.Write("Ingrese el nuevo nombre del producto: ");
        string nuevoNombre = Console.ReadLine();
        Console.Write("Ingrese la nueva descripción del producto: ");
        string nuevaDescripcion = Console.ReadLine();
        Console.Write("Ingrese el nuevo precio del producto: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal nuevoPrecio))
        {
            Console.WriteLine("Por favor, ingrese un precio válido.");
            return;
        }

        Console.Write("Ingrese el nuevo ID de la marca: ");
        if (!int.TryParse(Console.ReadLine(), out int nuevaMarcaId))
        {
            Console.WriteLine("Por favor, ingrese un ID de marca válido.");
            return;
        }

        Console.Write("Ingrese el nuevo ID de la categoría: ");
        if (!int.TryParse(Console.ReadLine(), out int nuevaCategoriaId))
        {
            Console.WriteLine("Por favor, ingrese un ID de categoría válido.");
            return;
        }

        EjecutarComando("UPDATE Producto SET nombre = @nombre, descripcion = @descripcion, precio = @precio, marca_id = @marcaId, categoria_id = @categoriaId WHERE id = @id", cmd =>
        {
            cmd.Parameters.AddWithValue("@nombre", nuevoNombre);
            cmd.Parameters.AddWithValue("@descripcion", nuevaDescripcion);
            cmd.Parameters.AddWithValue("@precio", nuevoPrecio);
            cmd.Parameters.AddWithValue("@marcaId", nuevaMarcaId);
            cmd.Parameters.AddWithValue("@categoriaId", nuevaCategoriaId);
            cmd.Parameters.AddWithValue("@id", id);
        });

        Console.WriteLine("Producto actualizado exitosamente.");
    }

    static void EliminarProducto()
    {
        Console.Write("Ingrese el ID del producto a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Por favor, ingrese un número válido.");
            return;
        }

        EjecutarComando("DELETE FROM Producto WHERE id = @id", cmd =>
        {
            cmd.Parameters.AddWithValue("@id", id);
        });

        Console.WriteLine("Producto eliminado exitosamente.");
    }


    static string EncriptarSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
