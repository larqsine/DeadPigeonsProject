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

    const handleLogin = async (username: string) => {
        setTransitioning(true);
        setTimeout(async () => {
            setIsLoggedIn(true);
            setIsAdmin(username.toLowerCase() === 'admin');
            setUsername(username);
            setShowBoxGrid(username.toLowerCase() !== 'admin'); // Show BoxGrid if not admin
            setTransitioning(false);

            try {
                // Fetch the playerId from the backend using the username
                const playerIdResponse = await fetch(`http://localhost:5229/api/player/username/${username}`);
                if (!playerIdResponse.ok) {
                    throw new Error('Failed to fetch player ID');
                }

                const playerId = await playerIdResponse.json();

                const balanceResponse = await fetch(`http://localhost:5229/api/player/${playerId}/balance`);
                if (!balanceResponse.ok) {
                    throw new Error('Failed to fetch player balance');
                }

                const balanceData = await balanceResponse.json();
                setBalance(balanceData);
            } catch (error) {
                console.error('Error fetching player data:', error);
                setBalance(null);
            }
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
