import React from 'react';
import './Principal.css';
import Sidebar from '../Components/Sidebar';
import Header from '../Components/Header';
import Content from '../Components/Content';

interface PrincipalProps {
  token: string;
  onLogout: () => void;
}

const Principal: React.FC<PrincipalProps> = ({onLogout, token}) => {
  return (
    <div className="princi">
      <Sidebar/>
      <div className="main-content">
        <Header onLogout={onLogout} token={token}/>
        <Content token={token}/>
      </div>
    </div>
  );
};

export default Principal;