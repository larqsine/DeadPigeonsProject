import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./NavBar.module.css";

interface NavbarProps {
    onPlayClick: () => void;
    username: string;
    onSignOut: () => void;
    isAdmin: boolean;
    onGoToAdminPage: () => void;
    playerId: string;
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
    const [transactionId, setTransactionId] = useState("");
    const dropdownRef = React.useRef<HTMLDivElement | null>(null);
    const transactionInputRef = React.useRef<HTMLInputElement | null>(null);

    const navigate = useNavigate();

    // Handle Add Balance button click
    const handleAddBalanceClick = () => {
        setIsAddBalanceOpen(true);
        setIsDropdownOpen(false);
    };

    const handleTransactionChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTransactionId(event.target.value);
    };

    const handleSubmitTransaction = () => {
        if (!transactionId) {
            alert("Please enter a valid transaction ID");
            return;
        }

        console.log("Transaction ID entered:", transactionId);
        alert(`Transaction number: "${transactionId}" has been sent.`);

        setTransactionId("");
        setIsAddBalanceOpen(false);
    };

    // Close the dropdown if clicked outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsDropdownOpen(false);
            }
            if (transactionInputRef.current && !transactionInputRef.current.contains(event.target as Node)) {
                setIsAddBalanceOpen(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, []);

    // Handle navigation events
    const handlePlayClick = () => {
        onPlayClick();
        navigate("/");
    };

    const handleGoToAdminPage = () => {
        onGoToAdminPage();
        navigate("/admin");
    };

    const handleSignOutClick = () => {
        onSignOut();
        navigate("/login");
    };

    return (
        <div className={styles.nav}>
            {/* Logo */}
            <div className={styles.logo}>
                <img src="/logo.png" alt="App Logo" className={styles.logo} />
            </div>

            {/* Center Buttons */}
            <ul className={styles.navButtons}>
                <li className={styles.navItem} onClick={handlePlayClick}>
                    Play
                </li>
                <li className={styles.navItem}>Board History</li>
                <li className={styles.navItem}>Current Winnings</li>
            </ul>

            {/* Balance Display */}
            <div className={styles.Balance}>Balance: {balance.toFixed(2)} DKK</div>

            {/* Username/Profile Dropdown */}
            <div
                className={styles.profileButton}
                onClick={() => setIsDropdownOpen(!isDropdownOpen)}
            >
                {username || "Guest"}
            </div>

            {isDropdownOpen && (
                <div ref={dropdownRef} className={styles.dropdownMenu}>
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
                <div className={styles.addBalanceContainer}>
                    <input
                        ref={transactionInputRef}
                        type="text"
                        placeholder="Enter Transaction ID"
                        value={transactionId}
                        onChange={handleTransactionChange}
                    />
                    <button onClick={handleSubmitTransaction}>Submit</button>
                </div>
            )}
        </div>
    );
};

export default NavBar;
