import React, { useState } from 'react';
import axios from 'axios';
import Swal from 'sweetalert2';
import './Login.css';

interface LoginProps {
    setToken: (token: string) => void;
}

const Login: React.FC<LoginProps> = ({ setToken }) => {
    const [usuario, setusuario] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [showPassword, setShowPassword] = useState(false); 

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        try {
            const response = await axios.post('https://localhost:7181/api/Usuarios/Login', { usuario, password });

            const token = response.data.token || response.data.jwt; 

            if (token) {
                setToken(token); 
            } else {
                console.error('Error de sesión');
            }

        } catch (error) {
            Swal.fire({
                icon: "error",
                title: "Error de Sesion",
                text: "Las credenciales no son validas"
              });
        }
    };
    
    const togglePasswordVisibility = () => {
        setShowPassword(prev => !prev); 
    };
    

    return (
        <div className='cont_login'>
            <div className='cont'>
            <div className='cont_login_children'>
                <form onSubmit={handleSubmit}> 
                <div className="mb-3">
                <h2 style={{textAlign:"center"}}>LOGIN</h2>
                <div style={{textAlign:"center"}}><img src="https://cdn-icons-png.flaticon.com/512/219/219986.png" alt="usr" width={100} /></div>
                <label style={{display:"block"}} htmlFor="exampleInputEmail1" className="form-label">Usuario</label>
                <input type="text" className="form-control" id="exampleInputEmail1" aria-describedby="emailHelp"
                 value={usuario}
                 onChange={(e) => setusuario(e.target.value)}
                 required
                 autoComplete='off'/>
                </div>
                <div className="mb-3">
                    <label htmlFor="exampleInputPassword1" className="form-label">Contraseña</label>
                    <input type={showPassword ? "text" : "password"} className="form-control" id="exampleInputPassword1"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                    autoComplete='off'/>
                </div>
                <div className="mb-3 form-check">
                    <input type="checkbox" className="form-check-input" id="exampleCheck1"
                    onChange={togglePasswordVisibility}/>
                    <label className="form-check-label" htmlFor="exampleCheck1">Ver</label>
                </div>
                <div style={{textAlign:"center"}}>
                    <button type="submit" className="btn btn-primary Iniciar">Iniciar Sesión</button>
                </div>
            </form>
            </div>
            <div className='cont2'>
                <img src="https://www.uaf.gob.do/images/SB2.png" alt="sb" />
            </div>
            </div>

        </div>
    );
};

export default Login;



