import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAtom } from "jotai";
import styles from "./NavBar.module.css";
import { playerIdAtom, balanceAtom } from "../Pages/PagesJotaiStore.ts"; 

interface NavbarProps {
    onPlayClick: () => void;
    username: string;
    onSignOut: () => void;
    isAdmin: boolean;
    onGoToAdminPage: () => void;
}

const NavBar: React.FC<NavbarProps> = ({
                                           onPlayClick,
                                           username,
                                           onSignOut,
                                           isAdmin,
                                           onGoToAdminPage,
                                       }) => {
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [isAddBalanceOpen, setIsAddBalanceOpen] = useState(false);
    const [desiredAmount, setDesiredAmount] = useState(""); 
    const [mobilePayNumber, setMobilePayNumber] = useState(""); 
    const [playerId] = useAtom(playerIdAtom); 
    const dropdownRef = React.useRef<HTMLDivElement | null>(null);
    const addBalanceRef = React.useRef<HTMLDivElement | null>(null);
    const hamburgerRef = React.useRef<HTMLDivElement | null>(null);
    const navigate = useNavigate();
    const [isNavOpen, setIsNavOpen] = useState(false);
    const [isMobileScreen, setIsMobileScreen] = useState(false);
    const[balance, setBalance] = useAtom(balanceAtom);

    const fetchBalance = async () => {
        if (!playerId) return;
        const token = localStorage.getItem("token");
        if (!token) {
            console.error("Authorization token is missing.");
            return;
        }
        
        try {
            const response = await fetch(`https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/player/${playerId}/balance`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const data = await response.json(); // Parse raw number response
                setBalance(data);
            } else {
                console.error("Failed to fetch balance");
            }
        } catch (error) {
            console.error("Error fetching balance:", error);
        }
    };
    
    useEffect(() => {
        if (playerId) fetchBalance();

        const intervalId = setInterval(() => {
            if (playerId) fetchBalance();
        }, 5000); // Poll every 5 seconds

        return () => clearInterval(intervalId);
    }, [playerId]);

    const handleSubmitTransaction = async () => {
        if (!desiredAmount || !mobilePayNumber) {
            alert("Please enter both the desired amount and MobilePay number.");
            return;
        }
        try {
            const response = await fetch(`https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Player/${playerId}/deposit`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    playerId,
                    amount: parseFloat(desiredAmount),
                    mobilePayNumber,
                }),
            });

            const result = await response.json();

            if (response.ok) {
                alert("Balance added successfully!");
                setDesiredAmount("");
                setMobilePayNumber("");
                setIsAddBalanceOpen(false);
            } else {
                alert(result.message || "Failed to add balance.");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("An error occurred while adding balance.");
        }
    };
    
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (
                dropdownRef.current &&
                !dropdownRef.current.contains(event.target as Node)
            ) {
                setIsDropdownOpen(false);
            }
            if (
                addBalanceRef.current &&
                !addBalanceRef.current.contains(event.target as Node)
            ) {
                setIsAddBalanceOpen(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, []);

    useEffect(() => {
        const handleResize = () => {
            setIsMobileScreen(window.innerWidth < 450);
        };
        handleResize();
        window.addEventListener("resize", handleResize);
        return () => {
            window.removeEventListener("resize", handleResize);
        };
    }, []);
    
    const handleAddBalanceClick = () => {
        setIsAddBalanceOpen(true);
        setIsDropdownOpen(false);
    };
    
    const handlePlayClick = () => {
        setIsNavOpen(false);
        onPlayClick();
        navigate("/play");
    };

    const handleGoToAdminPage = () => {
        setIsNavOpen(false);
        onGoToAdminPage();
        navigate("/admin");
    };

    const handleSignOutClick = () => {
        setIsNavOpen(false);
        onSignOut();
        navigate("/login");
    };
    
    const handleGoToTransactionsPage = () => {
        setIsNavOpen(false);
        navigate("/transactions");
    };
    
    const handleGoToBoardsHistoryPage = () => {
        setIsNavOpen(false);
        navigate("/board-history");
    }
    const handleGoToAllUserBoardsPage = () => {
        setIsNavOpen(false);
        navigate("/all-user-boards");
    }
    
    const toggleNav = () => {
        setIsNavOpen(!isNavOpen);
    };
    
    return (
        <div className={styles.nav}>
            {/* Logo */}
            <div className={styles.logo}>
                <img src="/logo.png" alt="App Logo" className={styles.logo}/>
            </div>

            {/* Hamburger Menu */}
            <div ref={hamburgerRef} className={styles.hamburger} onClick={toggleNav}>
                <div></div>
                <div></div>
                <div></div>
            </div>
            
            {/* Center Buttons */}
            <ul className={`${styles.navButtons} ${isNavOpen ? styles.show : ""}`}>
                <li className={styles.navItem} onClick={handlePlayClick}>
                    Play
                </li>
                <li className={styles.navItem} onClick={handleGoToBoardsHistoryPage}>
                    Board History
                </li>
                {isAdmin && (
                    <li className={styles.navItem} onClick={handleGoToTransactionsPage}>
                        Transactions
                    </li>
                )}
                {isAdmin && (
                    <li className={styles.navItem} onClick={handleGoToAllUserBoardsPage}>
                        All User Boards
                    </li>
                )}

                {/* Profile Button Inside Hamburger Menu */}
                {isMobileScreen && (
                    <li className={styles.navItem}>
                        {username || "Guest"}
                        <div
                            className={`${styles.dropdownMenu} ${
                                isMobileScreen ? styles.mobileDropdown : ""
                            }`}
                        >
                            {isAdmin ? (
                                <button className={styles.dropdownMenuButton} onClick={onGoToAdminPage}>
                                    Admin Page
                                </button>
                            ) : (
                                <button className={styles.dropdownMenuButton} onClick={handleAddBalanceClick}>
                                    Add Balance
                                </button>
                            )}
                            <button className={styles.dropdownMenuButton} onClick={onSignOut}>
                                Sign Out
                            </button>
                        </div>
                    </li>
                )}
            </ul>
            
            {/* Balance Display */}
            {!isAdmin && (
            <div className={styles.Balance}>Balance: {balance ? `${balance.toFixed(2)} DKK` : "Loading..."} </div>
            )}

            {/* Profile Dropdown */}
            <div className={styles.profileButton} onClick={() => setIsDropdownOpen(!isDropdownOpen)}>
                {username || "Guest"}
            </div>

            {isDropdownOpen && (
                <div
                    ref={dropdownRef}
                    className={styles.dropdownMenu}
                    onClick={(e) => e.stopPropagation()}
                >
                    {isAdmin ? (
                        <button className={styles.dropdownMenuButton} onClick={handleGoToAdminPage}>Admin Page</button>
                    ) : (
                        <button className={styles.dropdownMenuButton} onClick={handleAddBalanceClick}>Add
                            Balance</button>
                    )}
                    <button className={styles.dropdownMenuButton} onClick={handleSignOutClick}>Sign Out</button>
                </div>
            )}

            {/* Add Balance Input */}
            {isAddBalanceOpen && (
                <div ref={addBalanceRef} className={styles.addBalance}>
                    <div className={styles.amountInputContainer}>
                    <input
                            type="number"
                            placeholder="Enter desired amount"
                            value={desiredAmount}
                            onChange={(e) => setDesiredAmount(e.target.value)}
                        />
                    </div>
                    <div className={styles.mobilePayInputContainer}>
                        <input
                            type="text"
                            placeholder="Enter MobilePay number"
                            value={mobilePayNumber}
                            onChange={(e) => setMobilePayNumber(e.target.value)}
                        />
                    </div>
                    <button onClick={handleSubmitTransaction}>Submit</button>
                </div>
            )}
        </div>
    );
};

export default NavBar;
