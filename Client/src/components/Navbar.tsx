import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAtom } from "jotai"; // Import the useAtom hook
import styles from "./NavBar.module.css";
import { playerIdAtom } from "../Pages/PagesJotaiStore.ts"; // Ensure this is the correct path to your store

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
    const [desiredAmount, setDesiredAmount] = useState(""); // For desired amount
    const [mobilePayNumber, setMobilePayNumber] = useState(""); // For MobilePay number
    const [playerId] = useAtom(playerIdAtom); // Correctly access the atom using useAtom
    const dropdownRef = React.useRef<HTMLDivElement | null>(null);
    const navigate = useNavigate();

    const handleAddBalanceClick = () => {
        setIsAddBalanceOpen(true);
        setIsDropdownOpen(false);
    };

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

    // Close the dropdown if clicked outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsDropdownOpen(false);
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

    const handlePlayClick = () => {
        onPlayClick();
        navigate("/play");
    };

    const handleGoToAdminPage = () => {
        onGoToAdminPage();
        navigate("/admin");
    };

    const handleSignOutClick = () => {
        onSignOut();
        navigate("/login");
    };
    
    const handleGoToTransactionsPage = () => {
        navigate("/transactions");
    };
    
    const handleGoToBoardsHistoryPage = () => {
        navigate("/board-history");
    }
    
    return (
        <div className={styles.nav}>
            {/* Logo */}
            <div className={styles.logo}>
                <img src="/logo.png" alt="App Logo" className={styles.logo}/>
            </div>

            {/* Center Buttons */}
            <ul className={styles.navButtons}>
                <li className={styles.navItem} onClick={handlePlayClick}>
                    Play
                </li>
                <li className={styles.navItem} onClick={handleGoToBoardsHistoryPage}>
                    Board History
                </li>
                <li className={styles.navItem}>Current Winnings</li>
                <li className={styles.navItem} onClick={handleGoToTransactionsPage}>Transactions</li>
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
                    onClick={(e) => e.stopPropagation()} // Prevent click propagation
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
                <div className={styles.addBalance}>
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
