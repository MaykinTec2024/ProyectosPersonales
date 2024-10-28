// src/components/Content.tsx
import React, { useEffect, useState } from 'react';
import './Content.css';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import Swal from 'sweetalert2';

interface ContentProps {
  token: string;
}

interface Entidad {
  id:  string;
  nombre: string;
  tipo: string;
  direccion: string;
  ciudad: string;
  telefono: string;
  correoElectronico: string;
}

interface DecodedToken {
  rol: string;
}

const Content: React.FC<ContentProps> = ({token}) => {

  const [entities, setEntities] = useState<Entidad[]>([]);
  const [role, setRole] = useState<string>(''); 

  useEffect(() => {
    const fetchData = async () => {
        try {
            const response = await axios.get("https://localhost:7181/api/Entities", {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            setEntities(response.data);
            const decodedToken = jwtDecode<DecodedToken>(token);
            const userRole = decodedToken.rol; 
            setRole(userRole);
        } catch (err) {
            console.error('Error al obtener las entidades:', err);
        }
    };
    fetchData();
}, [entities, token]);

  const isAdmin = role === 'admin';

   //borrar candidate
   const deleteEntidad = async (id: string) => {
    try {
        await axios.delete(`https://localhost:7181/api/Entities/${id}`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        setEntities(entities.filter(entida => entida.id !== id));
    } catch (error) {
        Swal.fire({
            icon: "error",
            title: "Operación Fallida",
            text: "Error al eliminar el candidato"
          });
    }
};

  //nuevo Entidad
  const [newEntidad, setNewEntidad] = useState<Entidad>({
    id: "",
    nombre: '',
    tipo: '',
    direccion: "",
    ciudad: "",
    telefono: "",
    correoElectronico: ""
});

const createEntidad = async () => {
    try {
        const response = await axios.post('https://localhost:7181/api/Entities', newEntidad, {
            headers: { Authorization: `Bearer ${token}` }
        });
        setEntities([...entities, response.data]); 
        setNewEntidad({ id:"", nombre: '', tipo: '', direccion: "", ciudad: "", telefono:"", correoElectronico: ""});
        Swal.fire({
            position: "top-end",
            icon: "success",
            title: "Candidato Guardado",
            showConfirmButton: false,
            timer: 1200
          });
    } catch (error) {
        Swal.fire({
            icon: "error",
            title: "Operación Fallida",
            text: "Error al crear el candidato"
          });
    }
};


//actualizar

const [editingCandidate, setEditingCandidate] = useState<Entidad | null>(null);

const updateEntidad= async () => {
    if (editingCandidate) {
        try {
            const response = await axios.put(`https://localhost:7181/api/Entities/${editingCandidate.id}`, { ...newEntidad }, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setEntities(entities.map(entiti => (entiti.id === response.data.id ? response.data : entiti)));
            setEditingCandidate(null); 
            setNewEntidad({  id:"", nombre: '', tipo: '', direccion: "", ciudad: "", telefono:"", correoElectronico: ""});
        } catch (error) {
            Swal.fire({
                icon: "error",
                title: "Operación Fallida",
                text: "Error al actualizar el candidato"
              });
        }
    }
};

 const ObtenerDatos = async (Entidad: Entidad) => {
    try {
        const response = await axios.get(`https://localhost:7181/api/Entities/${Entidad.id}`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        const fetchedEntidad = response.data;
        setEditingCandidate(fetchedEntidad);
        setNewEntidad({
            id: fetchedEntidad.id,
            nombre: fetchedEntidad.nombre,
            tipo: fetchedEntidad.tipo,
            direccion: fetchedEntidad.direccion,
            ciudad: fetchedEntidad.ciudad,
            telefono: fetchedEntidad.telefono,
            correoElectronico: fetchedEntidad.correoElectronico,
        });
        
    } catch (error) {
        Swal.fire({
            icon: "error",
            title: "Operación Fallida",
            text: "Error al obtener los datos del candidato"
          });
    }
};


  return (
    <div className="content">
       <div className="detalle">
       <h2 className='title'>Registro de Candidatos</h2>
                <table className="table table-striped table-hover table-bordered">
                    <thead>
                        <tr className="table-primary">
                            <th>ID</th>
                            <th>Nombre</th>
                            <th>Tipo</th>
                            <th>Direccion</th>
                            <th>Ciudad</th>
                            <th>Telefono</th>
                            <th>Correo Electronico</th>
                            {isAdmin && <th>Acciones</th>}
                        </tr>
                    </thead>
                    <tbody>
                        {entities.map((item) => (
                            <tr key={item.id}>
                                <td>{item.id}</td>
                                <td>{item.nombre}</td>
                                <td>{item.tipo}</td>
                                <td>{item.direccion}</td>
                                <td>{item.ciudad}</td>
                                <td>{item.telefono}</td>
                                <td>{item.correoElectronico}</td>
                                {isAdmin && (
                                    <td style={{textAlign:'center'}}>
                                        <i className="fa-solid fa-square-pen edit"data-bs-toggle="modal" data-bs-target="#staticBackdrop2" onClick={()=>{
                                            ObtenerDatos(item)
                                        }}></i>
                                        <i className="fa-solid fa-trash-can borr" onClick={()=> {
                                            Swal.fire({
                                                title: "¿Desea Eliminar este Registro?",
                                                icon: "warning",
                                                showCancelButton: true,
                                                confirmButtonColor: "#3085d6",
                                                cancelButtonColor: "#d33",
                                                confirmButtonText: "Confirmar"
                                              }).then((result) => {
                                                if (result.isConfirmed) {
                                                    deleteEntidad(item.id)
                                                  Swal.fire({
                                                    title: "Registro Eliminiado",
                                                    icon: "success"
                                                  });
                                                }
                                              });
                                            }}></i>
                                    </td>
                                )}
                            </tr>
                        ))}
                    </tbody>
                </table>
                {isAdmin && (
                    <div style={{textAlign:"end"}}>
                        <button className="btn btn-primary" data-bs-toggle="modal" data-bs-target="#staticBackdrop"> Añadir</button>
                    </div>
                )}

              <div  className="modal fade" id="staticBackdrop" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex={-1} aria-labelledby="staticBackdropLabel" aria-hidden="true">
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5" id="exampleModalLabel">Agregar Entidad</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div className="modal-body">
                                <table>
                                    <tbody>
                                    <tr>
                                        <td>id</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.id}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, id: e.target.value})}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Nombre</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.nombre}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, nombre: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Tipo</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.tipo}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, tipo: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Dirección</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.direccion}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, direccion: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Ciudad</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.ciudad}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, ciudad: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Telefono</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.telefono}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, telefono: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Correo</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.correoElectronico}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, correoElectronico: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    </tbody>
                                </table>

                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-danger" data-bs-dismiss="modal" onClick={()=> setNewEntidad({  id:"", nombre: '', tipo: '', direccion: "", ciudad: "", telefono:"", correoElectronico: "" })}>Cerrar</button>
                            <button type="button" className="btn btn-primary" data-bs-dismiss="modal" onClick={()=>{
                               if (
                                !newEntidad.nombre ||
                                !newEntidad.tipo ||
                                !newEntidad.direccion ||
                                !newEntidad.ciudad ||
                                !newEntidad.telefono ||
                                !newEntidad.correoElectronico ||
                                !newEntidad.id
                            ) {
                                Swal.fire({
                                    icon: "error",
                                    title: "No se registro",
                                    text: "Completar todos los Campos por favor"
                                  });
                            } else {
                                createEntidad();
                            }
                                }}>Guardar</button>
                        </div>
                    </div>
                </div>
            </div>

            <div  className="modal fade" id="staticBackdrop2" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex={-1} aria-labelledby="staticBackdropLabel" aria-hidden="true">
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5" id="exampleModalLabel">Editar Entidad</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div className="modal-body">
                                <table>
                                   <tbody>
                                    <tr>
                                        <td>Nombre</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.nombre}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, nombre: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>tipo</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.tipo}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, tipo: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Dirección</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.direccion}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, direccion: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Ciudad</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.ciudad}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, ciudad: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Telefono</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.telefono}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, telefono: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Correo</td>
                                        <td>
                                            <input
                                                type="text"
                                                value={newEntidad.correoElectronico}
                                                onChange={(e) => setNewEntidad({ ...newEntidad, correoElectronico: e.target.value })}
                                                required
                                                className='form-control'
                                            />
                                        </td>
                                    </tr>
                                    </tbody>
                                </table>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-danger" data-bs-dismiss="modal" onClick={()=> setNewEntidad({ id:"", nombre: '', tipo: '', direccion: "", ciudad: "", telefono:"", correoElectronico: ""})}>Cerrar</button>
                            <button type="button" className="btn btn-primary" data-bs-dismiss="modal" onClick={()=>{
                                updateEntidad()
                                Swal.fire({
                                    position: "top-end",
                                    icon: "success",
                                    title: "Candidato Actualizado",
                                    showConfirmButton: false,
                                    timer: 1200
                                });
                                }}>Editar</button>
                        </div>
                    </div>
                </div>
            </div>

       </div>
    </div>
  );
};

export default Content;
