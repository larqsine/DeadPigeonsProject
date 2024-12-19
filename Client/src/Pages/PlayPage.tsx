import React, { useEffect } from 'react';
import { useAtom } from 'jotai';
import axios from 'axios';
import styles from './PlayPage.module.css';
import {
    selectedBoxesAtom,
    playerIdAtom,
    gameIdAtom,
    messageAtom,
    errorAtom,
    userAtom,
    autoPlayAtom, // Add this to manage AutoPlay state
} from './PagesJotaiStore.ts';

const PlayPage: React.FC = () => {
    const [selectedBoxes, setSelectedBoxes] = useAtom(selectedBoxesAtom);
    const [user] = useAtom(userAtom);
    const [playerId, setPlayerId] = useAtom(playerIdAtom);
    const [gameId, setGameId] = useAtom(gameIdAtom);
    const [message, setMessage] = useAtom(messageAtom);
    const [error, setError] = useAtom(errorAtom);
    const [autoPlay, setAutoPlay] = useAtom(autoPlayAtom);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const response = await axios.get(
                    `http://localhost:6329/api/Player/current`,
                    {
                        headers: {
                            Authorization: 'Bearer ' + user?.token,
                        },
                    }
                );
                setPlayerId(response.data.id);
            } catch (err) {
                setError('Failed to fetch player data');
            }
        };

        const fetchGameData = async () => {
            try {
                const response = await axios.get(
                    `http://localhost:6329/api/Games/active`
                );
                setGameId(response.data.gameId);
            } catch (err) {
                setError('Failed to fetch game data');
            }
        };

        fetchUserData();
        fetchGameData();
    }, [playerId, setPlayerId, setGameId, setError]);

    const handleBoxClick = (num: number) => {
        if (selectedBoxes.includes(num)) {
            setSelectedBoxes(selectedBoxes.filter((box) => box !== num));
        } else if (selectedBoxes.length < 8) {
            setSelectedBoxes([...selectedBoxes, num]);
        } else {
            alert('You can choose a maximum of 8 numbers');
        }
    };

    const handleAddBoard = async () => {
        if (selectedBoxes.length < 5 || selectedBoxes.length > 8) {
            alert('Please select between 5 and 8 numbers.');
            return;
        }

        const payload = {
            fieldsCount: selectedBoxes.length,
            numbers: selectedBoxes,
            gameId: gameId,
            remainingAutoplayWeeks: autoPlay ? 4 : 0, 
        };
        
        try {
            const response = await axios.post(
                `http://localhost:6329/api/Board/${playerId}/buy`,
                payload
            );
            setMessage(response.data.message || 'Board purchased successfully!');
            setSelectedBoxes([]);
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

    const getBoardPrice = () => {
        const prices: Record<number, string> = {
            5: '20 DKK',
            6: '40 DKK',
            7: '80 DKK',
            8: '160 DKK',
        };
        return prices[selectedBoxes.length] || '';
    };
    // Automatically clear the success message after 3 seconds
    useEffect(() => {
        if (message) {
            const timer = setTimeout(() => {
                setMessage('');
            }, 3000);
            return () => clearTimeout(timer); 
        }
    }, [message, setMessage]);

    return (
        <div className={styles.container}>
            <h1>Purchase Your Board</h1>

            <div className={styles.gridContainer}>
                {Array.from({length: 16}, (_, i) => (
                    <div key={i + 1}
                        className={`${styles.box} ${
                            selectedBoxes.includes(i + 1) ? styles.selected : ''
                        }`}
                        onClick={() => handleBoxClick(i + 1)}
                    >
                        {i + 1}
                    </div>
                ))}
            </div>

            {selectedBoxes.length >= 5 && selectedBoxes.length <= 8 && (
                <p className={styles.priceText}>
                    {selectedBoxes.length} numbers selected. Price: {getBoardPrice()}
                </p>
            )}

            <div className={styles.actionRow}>
                <div>
                    <button className={styles.actionButton} onClick={handleAddBoard} disabled={selectedBoxes.length < 5 || selectedBoxes.length > 8}>
                        Buy Board
                    </button>
                </div>
                <div>
                    <label className={styles.autoPlayLabel}>
                        <input type="checkbox" checked={autoPlay} onChange={() => setAutoPlay(!autoPlay)}/>
                        AutoPlay
                    </label>
                </div>
            </div>
            
            {message && message !== '' ? <p style={{color: 'green'}}>{message}</p> : null}
            {error && error !== '' ? <p style={{color: 'red'}}>{error}</p> : null}
        </div>
    );
};

export default PlayPage;
