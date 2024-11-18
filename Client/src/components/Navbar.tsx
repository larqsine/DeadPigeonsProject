import React from 'react';
import styles from './NavBar.module.css';

const NavBar: React.FC = () => {
    return (
        <div className={styles.nav}>
            {/* Logo */}
            <div className={styles.logo}>
                <img src="/images/logo.png" alt="logo" />
            </div>

            {/* Center Buttons */}
            <ul className={styles.navButtons}>
                <li className={styles.navItem}>Play</li>
                <li className={styles.navItem}>Board History</li>
                <li className={styles.navItem}>Admin Panel</li>
                <li className={styles.navItem}>Current Winnings</li>
            </ul>

            {/* Profile Button */}
            <div className={styles.profileButton}>Profile</div>
        </div>
    );
};

export default NavBar;
