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
    const [username, setUsername] = useState<string | null>(null);
    const [balance, setBalance] = useState<number | null>(null); // Add state for balance

    const fetchPlayerBalance = async (playerId: string) => {
        try {
            const response = await fetch(`/api/player/${playerId}/balance`);
            if (!response.ok) {
                throw new Error('Failed to fetch balance');
            }
            const data = await response.json();
            setBalance(data.balance);
        } catch (error) {
            console.error('Error fetching balance:', error);
            setBalance(null); // Handle error by setting balance to null
            alert("Error fetching balance, please try again.");
        }
    };


    const handleLogin = (username: string) => {
        setTransitioning(true);
        setTimeout(() => {
            setIsLoggedIn(true);
            setIsAdmin(username.toLowerCase() === 'admin');
            setUsername(username);
            setShowBoxGrid(username.toLowerCase() !== 'admin'); // Show BoxGrid if not admin
            const playerId = generatePlayerId(username); // Replace with actual player ID logic
            fetchPlayerBalance(playerId); // Fetch balance after login
            setTransitioning(false);
        }, 500);
    };

    const handleSignOut = () => {
        setIsLoggedIn(false);
        setUsername(null);
        setIsAdmin(false);
        setBalance(null); // Clear balance on sign-out
    };

    const handlePlayClick = () => {
        setShowBoxGrid(true); // When "Play" is clicked, show the BoxGrid
    };

    const handleGoToAdminPage = () => {
        setShowBoxGrid(false); // Hide BoxGrid and show AdminPage
    };

   
    const generatePlayerId = (username: string): string => {
        return username.toLowerCase(); 
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
                        <Navbar
                            onPlayClick={handlePlayClick}
                            username={username || 'Guest'} // Use 'Guest' if username is null
                            balance={balance !== null ? balance : 0} // Use default balance of 0 if balance is null
                            onSignOut={handleSignOut} // Pass sign-out function
                            isAdmin={isAdmin} // Pass isAdmin prop
                            onGoToAdminPage={handleGoToAdminPage} // Pass onGoToAdminPage function
                            playerId={generatePlayerId(username || 'Guest')} // Pass the playerId for balance fetching, default 'Guest'
                        />
                        {showBoxGrid ? <BoxGrid /> : <AdminPage />} {/* Conditional rendering */}
                    </>
                )}
            </div>
        </div>
    );
};

export default App;
