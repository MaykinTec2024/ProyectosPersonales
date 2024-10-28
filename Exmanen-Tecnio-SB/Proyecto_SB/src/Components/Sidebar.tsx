// src/components/Sidebar.tsx
import React from 'react';
import './Sidebar.css';


const Sidebar: React.FC= () => {
  return (
    <aside className="sidebar">
      <div className="logo">
        <div className='asa'>
          <img className='sb' src="logo.png" alt="Superintendencia de Bancos"/>
        </div>
        <h2 style={{textAlign:'left',fontWeight:'bold'}}>SUPERINTENDENCIA DE BANCOS</h2>
        <p style={{textAlign:'left',fontWeight:'lighter'}}>REPUBLICA DOMINICANA</p>
      </div>
      <nav>
        <ul>
          <li className='btn'><a className='me' href=""> <i className="fa-solid fa-house casa"></i>Inicio</a></li>
        </ul>
      </nav>
    </aside>
  );
};

export default Sidebar;
