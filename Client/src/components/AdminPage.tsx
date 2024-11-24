import React, { useState } from 'react';
import styles from './AdminPage.module.css';

const AdminPage: React.FC = () => {
    const [selectedWinningNumbers, setSelectedWinningNumbers] = useState<number[]>([]);

    const handleBoxClick = (num: number) => {
        if (selectedWinningNumbers.includes(num)) {
            // Remove box if already selected
            setSelectedWinningNumbers(selectedWinningNumbers.filter(box => box !== num));
        } else if (selectedWinningNumbers.length < 3) {
            // Add box if under the 3-box limit
            setSelectedWinningNumbers([...selectedWinningNumbers, num]);
        } else {
            alert('You can select a maximum of 3 winning numbers.');
        }
    };

    const handleSubmit = () => {
        alert(`Winning numbers are: ${selectedWinningNumbers.join(', ')}`);
    };

    return (
        <div className={styles.container}>
            <h1 className={styles.header}>Admin Page</h1>
            <p className={styles.subheader}>Select up to 3 winning numbers:</p>
            <div className={styles.gridContainer}>
                {Array.from({ length: 16 }, (_, i) => (
                    <div
                        key={i + 1}
                        className={`${styles.box} ${
                            selectedWinningNumbers.includes(i + 1) ? styles.selected : ''
                        }`}
                        onClick={() => handleBoxClick(i + 1)}
                    >
                        {i + 1}
                    </div>
                ))}
            </div>
            <button
                className={styles.actionButton}
                onClick={handleSubmit}
                disabled={selectedWinningNumbers.length !== 3}
            >
                Confirm Winning Numbers
            </button>
        </div>
    );
};

export default AdminPage;
