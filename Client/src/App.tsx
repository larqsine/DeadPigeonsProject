import React, { useState } from 'react';
import Navbar from './components/Navbar';
import BoxGrid from './components/BoxGrid';
import LoginPage from './components/LoginPage';
import styles from './App.module.css';

const App: React.FC = () => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [transitioning, setTransitioning] = useState(false);

    const handleLogin = () => {
        setTransitioning(true); // Start transition
        setTimeout(() => {
            setIsLoggedIn(true);
            setTransitioning(false); // End transition
        }, 500); // Match animation duration
    };

    return (
        <div className={styles.app}>
            <div
                className={`${styles.page} ${!isLoggedIn ? styles.active : ''} ${
                    transitioning ? styles.fadeOut : ''
                }`}
            >
                <LoginPage onLogin={handleLogin} />
            </div>
            <div
                className={`${styles.page} ${isLoggedIn ? styles.active : ''} ${
                    transitioning ? styles.fadeIn : ''
                }`}
            >
                {isLoggedIn && (
                    <>
                        <Navbar />
                        <BoxGrid />
                    </>
                )}
            </div>
        </div>
    );
};

export default App;
