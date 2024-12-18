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
    const [autoPlay, setAutoPlay] = useAtom(autoPlayAtom); // AutoPlay state from Jotai

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const response = await axios.get(
                    `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Player/current`,
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
                    `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Games/active`
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
            remainingAutoplayWeeks: autoPlay ? 4 : 0, // Add AutoPlay logic
        };

        console.log('PlayerId:', playerId);
        console.log('GameId:', gameId);
        console.log('Payload:', payload);

        try {
            const response = await axios.post(
                `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Board/${playerId}/buy`,
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

    return (
        <div className={styles.container}>
            <h1>Purchase Your Board</h1>

            <div className={styles.gridContainer}>
                {Array.from({length: 16}, (_, i) => (
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

            {selectedBoxes.length >= 5 && selectedBoxes.length <= 8 && (
                <p className={styles.priceText}>
                    {selectedBoxes.length} numbers selected. Price: {getBoardPrice()}
                </p>
            )}

            <div className={styles.actionRow}>
                <div>
                    <button
                        className={styles.actionButton}
                        onClick={handleAddBoard}
                        disabled={selectedBoxes.length < 5 || selectedBoxes.length > 8}
                    >
                        Add Board
                    </button>
                </div>
                <div>
                    <label className={styles.autoPlayLabel}>
                        <input
                            type="checkbox"
                            checked={autoPlay}
                            onChange={() => setAutoPlay(!autoPlay)}
                        />
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
