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
import AllUserBoardsPage from "./Pages/AllUserBoardsPage.tsx";
import styles from "./App.module.css";
import ChangePasswordPage from "./Pages/ChangePasswordPage.tsx";
import {selectedBoxesAtom} from "./Pages/PagesJotaiStore.ts"

const App: React.FC = () => {
    const [isLoggedIn, setIsLoggedIn] = useAtom(isLoggedInAtom);
    const [isAdmin, setIsAdmin] = useAtom(isAdminAtom);
    const [showBoxGrid, setShowBoxGrid] = useAtom(showBoxGridAtom);
    const [transitioning, setTransitioning] = useAtom(transitioningAtom);
    const [username, setUsername] = useAtom(usernameAtom);
    const [, setBalance] = useAtom(balanceAtom);
    const navigate = useNavigate();
    const [, setSelectedBoxes] = useAtom(selectedBoxesAtom);

    const handleLogin = async (username: string, roles: string[], passwordChangeRequired: boolean) => {
        setTransitioning(true);

        try {
            setIsLoggedIn(true);
            setUsername(username);

            const isAdminRole = roles.includes("admin");
            setIsAdmin(isAdminRole);
            setShowBoxGrid(!isAdminRole);

            if (passwordChangeRequired) {
                navigate("/change-password");
                return;
            }

            const token = localStorage.getItem("token");
            if (!token) throw new Error("Token is missing. Please log in again.");

            const playerIdResponse = await fetch(`https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/player/current`, {
                method: "GET",
                headers: { Authorization: `Bearer ${token}` },
            });

            if (!playerIdResponse.ok) throw new Error(`Failed to fetch player ID. Status: ${playerIdResponse.status}`);
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
        setSelectedBoxes([]);
        localStorage.removeItem("token");
    };

    const handlePlayClick = () => {
        if (!isAdmin) setShowBoxGrid(true);
    };

    const handleGoToAdminPage = () => setShowBoxGrid(false);

    return (
        <div className={styles.app}>
            {/* Navbar */}
            {isLoggedIn && (
                <Navbar
                    onPlayClick={handlePlayClick}
                    username={username || "Guest"}
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
                                    showBoxGrid && !isAdmin ? (
                                        <PlayPage />
                                    ) : (
                                        <Navigate to={isAdmin ? "/admin" : "/"} />
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
                            path="/all-user-boards"
                            element={isLoggedIn && isAdmin ? <AllUserBoardsPage /> : <Navigate to="/" />}
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
