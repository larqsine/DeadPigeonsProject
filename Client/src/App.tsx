import React, { useState } from 'react';
import Navbar from './components/Navbar';
import BoxGrid from './components/BoxGrid';
import AdminPage from './components/AdminPage';
import LoginPage from './components/LoginPage';
import styles from './App.module.css';

const App: React.FC = () => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [isAdmin, setIsAdmin] = useState(false);
    const [showBoxGrid, setShowBoxGrid] = useState(true); // To toggle between BoxGrid and AdminPage
    const [transitioning, setTransitioning] = useState(false);

    const handleLogin = (username: string) => {
        setTransitioning(true);
        setTimeout(() => {
            setIsLoggedIn(true);
            setIsAdmin(username.toLowerCase() === 'admin');
            setShowBoxGrid(username.toLowerCase() !== 'admin'); // Show BoxGrid if not admin
            setTransitioning(false);
        }, 500);
    };

    const handlePlayClick = () => {
        setShowBoxGrid(true); // When "Play" is clicked, show the BoxGrid
    };

    return (
        <div className={styles.app}>
            {/* Login Page */}
            <div
                className={`${styles.page} ${!isLoggedIn ? styles.active : ''} ${
                    transitioning ? styles.fadeOut : ''
                }`}
            >
                <LoginPage onLogin={handleLogin} />
            </div>

            {/* Main Application Page */}
            <div
                className={`${styles.page} ${isLoggedIn ? styles.active : ''} ${
                    transitioning ? styles.fadeIn : ''
                }`}
            >
                {isLoggedIn && (
                    <>
                        <Navbar onPlayClick={handlePlayClick} />
                        {showBoxGrid ? <BoxGrid /> : <AdminPage />} {/* Conditional rendering */}
                    </>
                )}
            </div>
        </div>
    );
};

export default App;
