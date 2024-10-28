import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Login from './views/Login';
import Principal from './views/Principal';

const App: React.FC = () => {
    const [token, setToken] = useState<string | null>(null);

    useEffect(() => {
        const savedToken = localStorage.getItem('token');
        if (savedToken) {
            setToken(savedToken);
        }
    }, []);

    const isLoggedIn = !!token; 

    const onLogout = () => {
        setToken(null); 
        localStorage.removeItem('token'); 
    };

    const handleLogin = (newToken: string) => {
        setToken(newToken);
        localStorage.setItem('token', newToken); 
    };

    return (
        <Router>
            <Routes>
                <Route path="/login" element={isLoggedIn ? <Navigate to="/principal" /> : <Login setToken={handleLogin} />} />
                <Route path="/principal" element={isLoggedIn ? <Principal token={token} onLogout={onLogout} /> : <Navigate to="/login" />} />
                <Route path="*" element={<Navigate to="/login" />} /> 
            </Routes>
        </Router>
    );
};

export default App;





