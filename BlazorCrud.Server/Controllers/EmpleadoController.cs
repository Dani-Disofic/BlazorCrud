using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorCrud.Server.Models;
using BlazorCrud.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorCrud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly DbcrudBlazorContext _dbContext;

        public EmpleadoController(DbcrudBlazorContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseAPI = new ResponseAPI<List<EmpleadoDTO>>();
            var listaEmpleadoDTO = new List<EmpleadoDTO>();

            try
            {
                foreach (var item in await _dbContext.Empleados.Include(d => d.IdDepartamentoNavigation).ToListAsync())
                {
                    listaEmpleadoDTO.Add(new EmpleadoDTO
                    {
                        IdEmpleado = item.IdEmpleado,
                        NombreCompleto = item.NombreCompleto,
                        IdDepartamento = item.IdDepartamento,
                        Sueldo = item.Sueldo,
                        FechaAlta = item.FechaAlta,
                        Departamento = new DepartamentoDTO()
                        {
                            IdDepartamento = item.IdDepartamentoNavigation.IdDepartamento,
                            Nombre = item.IdDepartamentoNavigation.Nombre
                        }

                    });
                }
                responseAPI.EsCorrecto = true;
                responseAPI.Valor = listaEmpleadoDTO;
            }
            catch (Exception ex)
            {
                responseAPI.EsCorrecto = false;
                responseAPI.Mensaje = ex.Message;
            }
            return Ok(responseAPI);
        }


        [HttpGet]
        [Route("Buscar/{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var responseAPI = new ResponseAPI<EmpleadoDTO>();
            var EmpleadoDTO = new EmpleadoDTO();
            try
            {
                var dbEmpleado = await _dbContext.Empleados.FirstOrDefaultAsync(x => x.IdEmpleado == id);

                if (dbEmpleado != null)
                {
                    EmpleadoDTO.IdEmpleado = dbEmpleado.IdEmpleado;
                    EmpleadoDTO.NombreCompleto = dbEmpleado.NombreCompleto;
                    EmpleadoDTO.IdDepartamento = dbEmpleado.IdDepartamento;
                    EmpleadoDTO.Sueldo = dbEmpleado.Sueldo;
                    EmpleadoDTO.FechaAlta = dbEmpleado.FechaAlta;

                    responseAPI.EsCorrecto = true;
                    responseAPI.Valor = EmpleadoDTO;
                }
                else
                {
                    responseAPI.EsCorrecto = false;
                    responseAPI.Mensaje = "No Encontrado";
                }



            }
            catch (Exception ex)
            {
                responseAPI.EsCorrecto = false;
                responseAPI.Mensaje = ex.Message;
            }
            return Ok(responseAPI);
        }


        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar(EmpleadoDTO empleado)
        {
            var responseAPI = new ResponseAPI<int>();

            try
            {
                var dbEmpleado = new Empleado
                {
                    NombreCompleto = empleado.NombreCompleto,
                    IdDepartamento = empleado.IdDepartamento,
                    Sueldo = empleado.Sueldo,
                    FechaAlta = empleado.FechaAlta,
                };

                _dbContext.Empleados.Add(dbEmpleado);
                await _dbContext.SaveChangesAsync();

                if (dbEmpleado.IdEmpleado != 0)
                {
                    responseAPI.EsCorrecto = true;
                    responseAPI.Valor = dbEmpleado.IdEmpleado;
                }
                else
                {
                    responseAPI.EsCorrecto = true;
                    responseAPI.Mensaje = "No guardado";
                }




            }
            catch (Exception ex)
            {
                responseAPI.EsCorrecto = false;
                responseAPI.Mensaje = ex.Message;
            }
            return Ok(responseAPI);
        }


        [HttpPut]
        [Route("Editar/{id}")]
        public async Task<IActionResult> Editar(EmpleadoDTO empleado, int id)
        {
            var responseAPI = new ResponseAPI<int>();

            try
            {


                var dbEmpleado = await _dbContext.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);



                if (dbEmpleado != null)
                {
                    dbEmpleado.NombreCompleto = empleado.NombreCompleto;
                    dbEmpleado.IdDepartamento = empleado.IdDepartamento;
                    dbEmpleado.Sueldo = empleado.Sueldo;
                    dbEmpleado.FechaAlta = empleado.FechaAlta;

                    _dbContext.Empleados.Update(dbEmpleado);
                    await _dbContext.SaveChangesAsync();

                    responseAPI.EsCorrecto = true;
                    responseAPI.Valor = dbEmpleado.IdEmpleado;
                }
                else
                {
                    responseAPI.EsCorrecto = true;
                    responseAPI.Mensaje = "Empleado no encontrado";
                }




            }
            catch (Exception ex)
            {
                responseAPI.EsCorrecto = false;
                responseAPI.Mensaje = ex.Message;
            }
            return Ok(responseAPI);
        }


        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar( int id)
        {
            var responseAPI = new ResponseAPI<int>();

            try
            {


                var dbEmpleado = await _dbContext.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);



                if (dbEmpleado != null)
                {
                    
                    _dbContext.Empleados.Remove(dbEmpleado);
                    await _dbContext.SaveChangesAsync();

                    responseAPI.EsCorrecto = true;
                    
                }
                else
                {
                    responseAPI.EsCorrecto = true;
                    responseAPI.Mensaje = "Empleado no encontrado";
                }




            }
            catch (Exception ex)
            {
                responseAPI.EsCorrecto = false;
                responseAPI.Mensaje = ex.Message;
            }
            return Ok(responseAPI);
        }
    }
}
