using Api_Viviendas.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api_Viviendas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Clientes : ControllerBase
    {
        public readonly string conn;
        public SqlConnection _conection;
        public SqlCommand _cmd;
        public SqlDataReader _reader;
        public DataTable data;

        public Clientes(IConfiguration configuration)
        {
            conn = configuration.GetConnectionString("conexion");

         }

        [HttpGet]

        public async Task<IEnumerable<MClientes>> get() {

            List<MClientes> Lclientes = new List<MClientes>();

            _conection = new SqlConnection(conn);
            _conection.Open();
            _cmd = _conection.CreateCommand();
            _cmd.CommandText = "Select * from clientes";
            _reader = await _cmd.ExecuteReaderAsync();

             while (await _reader.ReadAsync()) {
             
                var cliente = new MClientes();
                var props = cliente.GetType().GetProperties();
                foreach (var prop in props)
                {
                    prop.SetValue(cliente, _reader[prop.Name] , null);
                }

                Lclientes.Add(cliente);

            }
            _conection.Dispose();

            return Lclientes;
        }

        [HttpPost]

        public void post([FromBody] MClientes mClientes)
        {
            _conection = new SqlConnection(conn);
            _conection.Open();
            _cmd = _conection.CreateCommand();
            _cmd.CommandText = "insert into clientes (nombre , fecha_nac , genero , direccion , code_client ) values (@nombre,@fecha_nac,@genero,@direccion,@code_client)";
            var props = mClientes.GetType().GetProperties().Skip(1).ToList();
            foreach (var prop in props) {
                _cmd.Parameters.AddWithValue($"@{prop.Name}" , prop.GetValue(mClientes));
            }
            _cmd.ExecuteNonQuery();
            _conection.Dispose();
        }

        [HttpPut("{id}")]

        public void put([FromBody] MClientes mClientes , int id) {

            _conection = new SqlConnection(conn);
            _conection.Open();
            _cmd = _conection.CreateCommand();
            _cmd.CommandText = $"Update clientes set nombre = @nombre , fecha_nac = @fecha_nac , genero = @genero , direccion = @direccion , code_client = @code_client where id_clientes = {id}";
            var props = mClientes.GetType().GetProperties().ToList();
            foreach (var prop in props)
            {
                _cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(mClientes));
            }
            _cmd.ExecuteNonQuery();
            _conection.Dispose();
        }

        [HttpDelete("{id}")]

        public void delete(int id) {

            _conection = new SqlConnection(conn);
            _conection.Open();
            _cmd = _conection.CreateCommand();
            _cmd.CommandText = $"Delete from clientes where id_clientes = {id}";          
            _cmd.ExecuteNonQuery();
            _conection.Dispose();
        }
    }
}
