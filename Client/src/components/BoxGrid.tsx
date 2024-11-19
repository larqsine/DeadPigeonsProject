import React, { useState } from 'react';
import styles from './BoxGrid.module.css';

const BoxGrid: React.FC = () => {
    const [selectedBoxes, setSelectedBoxes] = useState<number[]>([]);

    const handleBoxClick = (num: number) => {
        if (selectedBoxes.includes(num)) {
            // If already selected, remove it from the list
            setSelectedBoxes(selectedBoxes.filter(box => box !== num));
        } else if (selectedBoxes.length < 8) {
            // Add the box if under the 8-box limit
            setSelectedBoxes([...selectedBoxes, num]);
        } else {
            // Alert when attempting to select more than 8 boxes
            alert('You can choose a maximum of 8 numbers');
        }
    };

    const handleButtonClick = () => {
        alert(`You selected boxes: ${selectedBoxes.join(', ')}`);
    };

    // Determine board price based on selected boxes
    const getBoardPrice = () => {
        const prices: Record<number, string> = {
            5: "20 DKK",
            6: "40 DKK",
            7: "80 DKK",
            8: "160 DKK",
        };
        return prices[selectedBoxes.length] || "";
    };

    return (
        <div className={styles.container}>
            <div className={styles.gridContainer}>
                {Array.from({ length: 16 }, (_, i) => (
                    <div
                        key={i + 1}
                        className={`${styles.box} ${
                            selectedBoxes.includes(i + 1) ? styles.selected : ''
                        }`}
                        onClick={() => handleBoxClick(i + 1)}
                    >
                        {i + 1}
                    </div>
                ))}
            </div>
            <button
                className={styles.actionButton}
                onClick={handleButtonClick}
                disabled={selectedBoxes.length < 5 || selectedBoxes.length > 8}
            >
                Submit
            </button>
            {selectedBoxes.length >= 5 && selectedBoxes.length <= 8 && (
                <p className={styles.priceText}>
                    {selectedBoxes.length} Board Price: {getBoardPrice()}
                </p>
            )}
        </div>
    );
};

export default BoxGrid;
