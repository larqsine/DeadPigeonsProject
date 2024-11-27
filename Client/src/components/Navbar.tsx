import React from 'react';
import styles from './NavBar.module.css';

interface NavbarProps {
    onPlayClick: () => void; // Add the prop to handle Play button click
}

const NavBar: React.FC<NavbarProps> = ({ onPlayClick }) => {
    return (
        <div className={styles.nav}>
            {/* Logo */}
            <div className={styles.logo}>
                <img src="/logo.png" alt="App Logo" className={styles.logo} />
            </div>

            {/* Center Buttons */}
            <ul className={styles.navButtons}>
                <li className={styles.navItem} onClick={onPlayClick}>Play</li>
                <li className={styles.navItem}>Board History</li>
                <li className={styles.navItem}>Current Winnings</li>
            </ul>

            {/* Profile Button */}
            <div className={styles.profileButton}>Profile</div>
        </div>
    );
};

export default NavBar;
