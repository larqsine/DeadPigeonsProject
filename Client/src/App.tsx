import React, { useEffect } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import { useAtom } from "jotai";
import {
    isLoggedInAtom,
    isAdminAtom,
    showBoxGridAtom,
    transitioningAtom,
    usernameAtom,
    balanceAtom,
    passwordChangeRequiredAtom,
} from "./AppJotaiStore";
import Navbar from "./components/Navbar";
import AdminPage from "./Pages/AdminPage";
import LoginPage from "./Pages/LoginPage";
import PlayPage from "./Pages/PlayPage";
import styles from "./App.module.css";
import ChangePasswordPage from "./Pages/ChangePasswordPage";
import { useNavigate } from "react-router-dom";

const App: React.FC = () => {
    const [isLoggedIn, setIsLoggedIn] = useAtom(isLoggedInAtom);
    const [isAdmin, setIsAdmin] = useAtom(isAdminAtom);
    const [showBoxGrid, setShowBoxGrid] = useAtom(showBoxGridAtom);
    const [transitioning, setTransitioning] = useAtom(transitioningAtom);
    const [username, setUsername] = useAtom(usernameAtom);
    const [balance, setBalance] = useAtom(balanceAtom);
    const [passwordChangeRequired, setPasswordChangeRequired] = useAtom(passwordChangeRequiredAtom);
    const navigate = useNavigate();

    // Check localStorage for login state on mount
    useEffect(() => {
        const storedIsLoggedIn = localStorage.getItem("isLoggedIn") === "true";
        const storedUsername = localStorage.getItem("username");
        const storedIsAdmin = localStorage.getItem("isAdmin") === "true";
        const storedPasswordChangeRequired = localStorage.getItem("passwordChangeRequired") === "true";

        if (storedIsLoggedIn && storedUsername) {
            setIsLoggedIn(true);
            setUsername(storedUsername);
            setIsAdmin(storedIsAdmin);
            setPasswordChangeRequired(storedPasswordChangeRequired);
            setShowBoxGrid(storedIsAdmin !== true);
        } else {
            setIsLoggedIn(false);
        }
    }, []);

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

                // Persist login state to localStorage
                localStorage.setItem("isLoggedIn", "true");
                localStorage.setItem("username", username);
                localStorage.setItem("isAdmin", username.toLowerCase() === "admin" ? "true" : "false");
                localStorage.setItem("passwordChangeRequired", passwordChangeRequired.toString());

                if (passwordChangeRequired) {
                    navigate("/change-password");
                    return;
                } else {
                    navigate("/");
                }
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
        setPasswordChangeRequired(false);

        // Clear localStorage on sign out
        localStorage.removeItem("isLoggedIn");
        localStorage.removeItem("username");
        localStorage.removeItem("isAdmin");
        localStorage.removeItem("passwordChangeRequired");
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
                    playerId={username?.toLowerCase() || "guest"}
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
                        <Route
                            path="/change-password"
                            element={isLoggedIn && passwordChangeRequired ? (
                                <ChangePasswordPage />
                            ) : (
                                <Navigate to="/" />
                            )}
                        />
                        <Route path="*" element={<Navigate to={isLoggedIn ? "/" : "/login"} />} />
                    </Routes>
                </div>
            </div>
        </div>
    );
};

export default App;
