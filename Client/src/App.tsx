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

    // Fetch player balance from backend
    const fetchPlayerBalance = async (userId: string) => {
        try {
            const response = await fetch(`/api/player/${userId}`);
            const data = await response.json();
            setBalance(data.balance); // Assuming the API returns the balance in the 'balance' field
        } catch (error) {
            console.error('Error fetching balance:', error);
        }
    };

    // Handle login logic
    const handleLogin = (username: string) => {
        setTransitioning(true);
        setTimeout(() => {
            setIsLoggedIn(true);
            setIsAdmin(username.toLowerCase() === 'admin');
            setUsername(username);
            setShowBoxGrid(username.toLowerCase() !== 'admin'); // Show BoxGrid if not admin
            fetchPlayerBalance(username); // Fetch balance after login
            setTransitioning(false);
        }, 500);
    };

    // Handle sign out logic
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
        // Logic for navigating to Admin page
        setShowBoxGrid(false); // Hide BoxGrid and show AdminPage
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
                            username={username}
                            balance={balance} // Pass balance to Navbar
                            onSignOut={handleSignOut} // Pass sign-out function
                            isAdmin={isAdmin} // Pass isAdmin prop
                            onGoToAdminPage={handleGoToAdminPage} // Pass onGoToAdminPage function
                        />
                        {showBoxGrid ? <BoxGrid /> : <AdminPage />} {/* Conditional rendering */}
                    </>
                )}
            </div>
        </div>
    );
};

export default App;
