import React from 'react';
import { useAtom } from 'jotai';
import {
    isLoggedInAtom,
    isAdminAtom,
    showBoxGridAtom,
    transitioningAtom,
    usernameAtom,
    balanceAtom,
} from './AppJotaiStore.ts'; // Import your atoms
import Navbar from './components/Navbar';
import BoxGrid from './components/BoxGrid';
import AdminPage from './components/AdminPage';
import LoginPage from './components/LoginPage';
import styles from './App.module.css';

const App: React.FC = () => {
    const [isLoggedIn, setIsLoggedIn] = useAtom(isLoggedInAtom);
    const [isAdmin, setIsAdmin] = useAtom(isAdminAtom);
    const [showBoxGrid, setShowBoxGrid] = useAtom(showBoxGridAtom);
    const [transitioning, setTransitioning] = useAtom(transitioningAtom);
    const [username, setUsername] = useAtom(usernameAtom);
    const [balance, setBalance] = useAtom(balanceAtom);

    const handleLogin = async (username: string) => {
        setTransitioning(true);
        setTimeout(async () => {
            setIsLoggedIn(true);
            setIsAdmin(username.toLowerCase() === 'admin');
            setUsername(username);
            setShowBoxGrid(username.toLowerCase() !== 'admin');
            setTransitioning(false);

            try {
                const playerIdResponse = await fetch(`http://localhost:6329/api/player/username/${username}`);
                if (!playerIdResponse.ok) {
                    throw new Error('Failed to fetch player ID');
                }
                const playerId = await playerIdResponse.json();

                const balanceResponse = await fetch(`http://localhost:6329/api/player/${playerId}/balance`);
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
        setBalance(null);
    };

    const handlePlayClick = () => setShowBoxGrid(true);
    const handleGoToAdminPage = () => setShowBoxGrid(false);

    const generatePlayerId = (username: string): string => username.toLowerCase();

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
                            username={username || 'Guest'}
                            balance={balance !== null ? balance : 0}
                            onSignOut={handleSignOut}
                            isAdmin={isAdmin}
                            onGoToAdminPage={handleGoToAdminPage}
                            playerId={generatePlayerId(username || 'Guest')}
                        />
                        {showBoxGrid ? <BoxGrid /> : <AdminPage />}
                    </>
                )}
            </div>
        </div>
    );
};

export default App;
