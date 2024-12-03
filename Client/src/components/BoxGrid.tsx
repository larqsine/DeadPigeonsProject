import React, { useState, useEffect } from 'react';
import axios from 'axios'; // Import AxiosError
import styles from './BoxGrid.module.css';

const BoxGrid: React.FC = () => {
    const [selectedBoxes, setSelectedBoxes] = useState<number[]>([]);
    const [playerId, setPlayerId] = useState<string>('');
    const [gameId, setGameId] = useState<string>('');
    const [message, setMessage] = useState<string>(''); // Ensure message is always a string
    const [error, setError] = useState<string>('');

    // Fetch PlayerId and GameId when the component is mounted
    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const response = await axios.get(`/api/Player/${playerId}`);
                setPlayerId(response.data.playerId);
                setGameId(response.data.gameId);
            } catch (err) {
                setError('Failed to fetch player data');
            }
        };

        fetchUserData();
    }, []);

    const handleBoxClick = (num: number) => {
        if (selectedBoxes.includes(num)) {
            // If already selected, remove it from the list
            setSelectedBoxes(selectedBoxes.filter((box) => box !== num));
        } else if (selectedBoxes.length < 8) {
            // Add the box if under the 8-box limit
            setSelectedBoxes([...selectedBoxes, num]);
        } else {
            // Alert when attempting to select more than 8 boxes
            alert('You can choose a maximum of 8 numbers');
        }
    };

    const handleAddBoard = async () => {
        if (selectedBoxes.length < 5 || selectedBoxes.length > 8) {
            alert('Please select between 5 and 8 numbers.');
            return;
        }

        const payload = {
            PlayerId: playerId,
            Boards: [
                {
                    GameId: gameId,
                    PlayerId: playerId,
                    Numbers: selectedBoxes.join(','), // Convert selected numbers to a comma-separated string
                    FieldsCount: selectedBoxes.length,
                    Autoplay: false, // You can make this configurable if needed
                },
            ],
        };

        try {
            const response = await axios.post('/api/Board/purchase', payload);
            setMessage(response.data.message || 'Board purchased successfully!');
            setSelectedBoxes([]); // Clear selected numbers after purchase
        } catch (err) {
            if (axios.isAxiosError(err)) {
                console.error('Error Status:', err.response?.status);
                console.error('Error Response:', err.response?.data);
                setError(err.response?.data?.message || 'An error occurred during the purchase.');
            } else {
                console.error('Unexpected Error:', err);
                setError('An unexpected error occurred.');
            }
        }
    };


    // Determine board price based on selected boxes
    const getBoardPrice = () => {
        const prices: Record<number, string> = {
            5: '20 DKK',
            6: '40 DKK',
            7: '80 DKK',
            8: '160 DKK',
        };
        return prices[selectedBoxes.length] || '';
    };

    return (
        <div className={styles.container}>
            <h1>Purchase Your Board</h1>

            <div className={styles.gridContainer}>
                {/* Grid of selectable boxes */}
                {Array.from({ length: 16 }, (_, i) => (
                    <div
                        key={i + 1}
                        className={`${styles.box} ${selectedBoxes.includes(i + 1) ? styles.selected : ''}`}
                        onClick={() => handleBoxClick(i + 1)}
                    >
                        {i + 1}
                    </div>
                ))}
            </div>

            {/* Show the price only if the user selects between 5 and 8 numbers */}
            {selectedBoxes.length >= 5 && selectedBoxes.length <= 8 && (
                <p className={styles.priceText}>
                    {selectedBoxes.length} numbers selected. Price: {getBoardPrice()}
                </p>
            )}

            {/* Add Board button */}
            <button
                className={styles.actionButton}
                onClick={handleAddBoard}
                disabled={selectedBoxes.length < 5 || selectedBoxes.length > 8}
            >
                Add Board
            </button>

            {/* Show success or error message */}
            {message && message !== '' ? (
                <p style={{ color: 'green' }}>{message}</p>
            ) : null} {/* Ensure message is always a string */}

            {error && error !== '' ? (
                <p style={{ color: 'red' }}>{error}</p>
            ) : null} {/* Ensure error is always a string */}
        </div>
    );
};

export default BoxGrid;
