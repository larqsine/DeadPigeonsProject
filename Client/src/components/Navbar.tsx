import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAtom } from "jotai";
import styles from "./NavBar.module.css";
import { playerIdAtom } from "../Pages/PagesJotaiStore.ts"; 

interface NavbarProps {
    onPlayClick: () => void;
    username: string;
    onSignOut: () => void;
    isAdmin: boolean;
    onGoToAdminPage: () => void;
    balance: number;
}

const NavBar: React.FC<NavbarProps> = ({
                                           onPlayClick,
                                           username,
                                           onSignOut,
                                           isAdmin,
                                           onGoToAdminPage,
                                           balance,
                                       }) => {
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [isAddBalanceOpen, setIsAddBalanceOpen] = useState(false);
    const [desiredAmount, setDesiredAmount] = useState(""); 
    const [mobilePayNumber, setMobilePayNumber] = useState(""); 
    const [playerId] = useAtom(playerIdAtom); 
    const dropdownRef = React.useRef<HTMLDivElement | null>(null);
    const addBalanceRef = React.useRef<HTMLDivElement | null>(null);
    const navigate = useNavigate();
    const [isNavOpen, setIsNavOpen] = useState(false);
    const [isMobileScreen, setIsMobileScreen] = useState(false);
    
    
    
    const handleSubmitTransaction = async () => {
        if (!desiredAmount || !mobilePayNumber) {
            alert("Please enter both the desired amount and MobilePay number.");
            return;
        }
        try {
            const response = await fetch(`https://server-587187818392.europe-west1.run.app/api/Player/${playerId}/deposit`, {
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
                setDesiredAmount(""); // Clear inputs
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

    // Close the dropdown menus if clicked outside
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
        console.log("Player ID in NavBar:", playerId);
    }, [playerId]);

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
        closeNav();
        onPlayClick();
        navigate("/play");
    };

    const handleGoToAdminPage = () => {
        closeNav();
        onGoToAdminPage();
        navigate("/admin");
    };

    const handleSignOutClick = () => {
        closeNav();
        onSignOut();
        navigate("/login");
    };
    
    const handleGoToTransactionsPage = () => {
        closeNav();
        navigate("/transactions");
    };
    
    const handleGoToBoardsHistoryPage = () => {
        closeNav();
        navigate("/board-history");
    }
    
    const toggleNav = () => {
        setIsNavOpen(!isNavOpen);
    };
    
    const closeNav = () => {
        setIsNavOpen(false); // Close hamburger menu
    };
    
    return (
        <div className={styles.nav}>
            {/* Logo */}
            <div className={styles.logo}>
                <img src="/logo.png" alt="App Logo" className={styles.logo}/>
            </div>

            {/* Hamburger Menu */}
            <div className={styles.hamburger} onClick={toggleNav}>
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

                {/* Profile Button Inside Hamburger Menu */}
                {isMobileScreen && (
                    <li className={`${styles.navItem} ${styles.profileButton}`}>
                        <div onClick={() => setIsDropdownOpen(!isDropdownOpen)}>
                            {username || "Guest"}
                        </div>
                        {isDropdownOpen && (
                            <div className={styles.dropdownMenu}>
                                {isAdmin && (
                                    <button onClick={handleGoToAdminPage}>Admin Page</button>
                                )}
                                <button onClick={handleSignOutClick}>Sign Out</button>
                            </div>
                        )}
                    </li>
                )}
            </ul>

            {/* Balance Display */}
            {!isAdmin && (
                <div className={styles.Balance}>Balance: {balance.toFixed(2)} DKK</div>
            )}


            {/* Username/Profile Dropdown */}
            <div
                className={styles.profileButton}
                onClick={() => setIsDropdownOpen(!isDropdownOpen)}
            >
                {username || "Guest"}
            </div>

            {isDropdownOpen && (
                <div
                    ref={dropdownRef}
                    className={styles.dropdownMenu}
                    onClick={(e) => e.stopPropagation()}
                >
                    {isAdmin ? (
                        <button onClick={handleGoToAdminPage}>Admin Page</button>
                    ) : (
                        <button onClick={handleAddBalanceClick}>Add Balance</button>
                    )}
                    <button onClick={handleSignOutClick}>Sign Out</button>
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
