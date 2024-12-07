import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { useAtom } from "jotai";
import {
    isLoggedInAtom,
    isAdminAtom,
    showBoxGridAtom,
    transitioningAtom,
    usernameAtom,
    balanceAtom,
} from "./AppJotaiStore";
import Navbar from "./components/Navbar";
import AdminPage from "./Pages/AdminPage";
import LoginPage from "./Pages/LoginPage";
import PlayPage from "./Pages/PlayPage";
import styles from "./App.module.css";

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
            setIsAdmin(username.toLowerCase() === "admin");
            setUsername(username);
            setShowBoxGrid(username.toLowerCase() !== "admin");
            setTransitioning(false);

            try {
                const playerIdResponse = await fetch(`http://localhost:6329/api/player/username/${username}`);
                if (!playerIdResponse.ok) throw new Error("Failed to fetch player ID");
                const playerId = await playerIdResponse.json();

                const balanceResponse = await fetch(`http://localhost:6329/api/player/${playerId}/balance`);
                if (!balanceResponse.ok) throw new Error("Failed to fetch player balance");

                const balanceData = await balanceResponse.json();
                setBalance(balanceData);
            } catch (error) {
                console.error("Error fetching player data:", error);
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

    return (
        <Router>
            <div className={styles.app}>
                {/* Navbar */}
                {isLoggedIn && (
                    <Navbar
                        onPlayClick={handlePlayClick}
                        username={username || "Guest"}
                        balance={balance !== null ? balance : 0}
                        onSignOut={handleSignOut}
                        isAdmin={isAdmin}
                        onGoToAdminPage={handleGoToAdminPage}
                        playerId={username?.toLowerCase() || "guest"}
                    />
                )}

                {/* Main Content */}
                <div className={styles.mainContent}>
                    {/* Routes with Page Transition Styling */}
                    <div
                        className={`${styles.page} ${!isLoggedIn ? styles.active : ""} ${
                            transitioning ? styles.fadeOut : ""
                        }`}
                    >
                        <Routes>
                            <Route
                                path="/login"
                                element={!isLoggedIn ? <LoginPage onLogin={handleLogin} /> : <Navigate to="/" />}
                            />
                        </Routes>
                    </div>

                    <div
                        className={`${styles.page} ${isLoggedIn ? styles.active : ""} ${
                            transitioning ? styles.fadeIn : ""
                        }`}
                    >
                        <Routes>
                            <Route
                                path="/"
                                element={
                                    isLoggedIn ? (
                                        showBoxGrid ? (
                                            <PlayPage />
                                        ) : (
                                            <Navigate to="/admin" />
                                        )
                                    ) : (
                                        <Navigate to="/login" />
                                    )
                                }
                            />
                            <Route
                                path="/admin"
                                element={isLoggedIn && isAdmin ? <AdminPage /> : <Navigate to="/" />}
                            />
                            <Route path="*" element={<Navigate to={isLoggedIn ? "/" : "/login"} />} />
                        </Routes>
                    </div>
                </div>
            </div>
        </Router>
    );
};

export default App;
