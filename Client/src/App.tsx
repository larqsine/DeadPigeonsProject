import React from "react";
import {Routes, Route, Navigate, useNavigate} from "react-router-dom";
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
import TransactionPage from "./Pages/TransactionPage.tsx";
import BoardHistoryPage from "./Pages/BoardsHistoryPage.tsx";
import styles from "./App.module.css";
import ChangePasswordPage from "./Pages/ChangePasswordPage.tsx";

const App: React.FC = () => {
    const [isLoggedIn, setIsLoggedIn] = useAtom(isLoggedInAtom);
    const [isAdmin, setIsAdmin] = useAtom(isAdminAtom);
    const [showBoxGrid, setShowBoxGrid] = useAtom(showBoxGridAtom);
    const [transitioning, setTransitioning] = useAtom(transitioningAtom);
    const [username, setUsername] = useAtom(usernameAtom);
    const [balance, setBalance] = useAtom(balanceAtom);
    const navigate = useNavigate();



    const handleLogin = async (username: string, roles: string[], passwordChangeRequired: boolean) => {
        setTransitioning(true);

        try {
            setIsLoggedIn(true);
            setUsername(username);

            const isAdminRole = roles.includes("admin");
            setIsAdmin(isAdminRole);
            setShowBoxGrid(!isAdminRole);

            if (passwordChangeRequired) {
                // Redirect to change-password page
                navigate("/change-password");
                return;
            }

                const token = localStorage.getItem('token');


            if (!token) throw new Error("Token is missing. Please log in again.");

            console.log("Fetching player ID using token:", token);


            const playerIdResponse = await fetch(`https://server-587187818392.europe-west1.run.app/api/player/current`, {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            console.log("Player ID response status:", playerIdResponse.status);

            if (!playerIdResponse.ok) {
                const errorResponse = await playerIdResponse.json();
                console.error("Player ID response error:", errorResponse);
                throw new Error(`Failed to fetch player ID. Status: ${playerIdResponse.status}`);
            }

            const { id: playerId } = await playerIdResponse.json();
            console.log("Fetched Player ID:", playerId);

            const balanceResponse = await fetch(`https://server-587187818392.europe-west1.run.app/api/player/${playerId}/balance`, {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                },
            });

            console.log("Balance response status:", balanceResponse.status);

            if (!balanceResponse.ok) {
                const errorResponse = await balanceResponse.json();
                console.error("Balance response error:", errorResponse);
                throw new Error(`Failed to fetch player balance. Status: ${balanceResponse.status}`);
            }

            const balanceData = await balanceResponse.json();
            console.log("Fetched Balance:", balanceData);
            setBalance(balanceData);
        } catch (error) {
            console.error("Error during login process:", error);
            setBalance(null);
        } finally {
            setTransitioning(false);
        }
    };


    const handleSignOut = () => {
        setIsLoggedIn(false);
        setUsername(null);
        setIsAdmin(false);
        setBalance(null);
        localStorage.removeItem("token");
    };

    const handlePlayClick = () => setShowBoxGrid(true);
    const handleGoToAdminPage = () => setShowBoxGrid(false);

    return (
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
                    />
                )}

                {/* Main Content */}
                <div className={styles.mainContent}>
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
                                path="/board-history"
                                element={isLoggedIn ? <BoardHistoryPage /> : <Navigate to="/login" />}
                            />
                            <Route
                                path="/admin"
                                element={isLoggedIn && isAdmin ? <AdminPage /> : <Navigate to="/" />}
                            />
                            <Route
                                path="/transactions"
                                element={isLoggedIn && isAdmin ? <TransactionPage /> : <Navigate to="/" />}
                            />

                            <Route
                                path="/change-password"
                                element={<ChangePasswordPage />}
                            />

                            <Route path="*" element={<Navigate to={isLoggedIn ? "/" : "/login"} />} />
                        </Routes>
                    </div>
                </div>
            </div>
    );
};

export default App;
