// src/components/Header.tsx
import React, { useEffect, useState } from 'react';
import './Header.css';
import { jwtDecode } from 'jwt-decode';

interface HeaderProps {
  onLogout: () => void;
  token: string;
}

interface DecodedToken {
  usuarioNombre: string;
}

const Header: React.FC<HeaderProps> = ({ onLogout, token }) => {
  const [usuario, setUsuario] = useState<string>('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        const decodedToken = jwtDecode<DecodedToken>(token);
        const user = decodedToken.usuarioNombre;
        console.log(user);
        setUsuario(user);
      } catch (err) {
        console.error('Error al obtener el usuario:', err);
      }
    };
    fetchData();
  }, [token]);

  return (
    <header className="header">
      <h2 style={{ display: 'inline', paddingTop:6 }}>Inicio</h2>
      <div className="cerrar">
        <div>
          <i style={{marginLeft:10, marginRight:12}} className="fa-solid fa-user"></i>
          <span>{usuario}</span>
        </div>
        <button className="btn-Logout cerr" onClick={onLogout}>Cerrar Sesión</button>
      </div>
    </header>
  );
};

export default Header;
