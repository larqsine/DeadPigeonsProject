import React, { useEffect, useState } from 'react';
import { useAtom } from 'jotai';
import axios from 'axios';
import styles from './BoardsHistoryPage.module.css';
import {
    userAtom,
    boardHistoryAtom,
    gameIdAtom,
    errorAtom,
} from './PagesJotaiStore.ts';

interface Game {
    id: string;
    startDate: string;
    endDate: string | null;
    isClosed: boolean;
    winningNumbers: number[] | null;
}

interface Board {
    id: string;
    numbers: string;
    fieldsCount: number;
    cost: number;
    createdAt: string;
    isWinning: boolean;
}

const BoardsHistoryPage: React.FC = () => {
    const [user] = useAtom(userAtom); // Get the current user
    const [boards, setBoards] = useAtom(boardHistoryAtom); // Boards for the current game
    const [activeGameId] = useAtom(gameIdAtom); // Active game ID
    const [previousGames, setPreviousGames] = useState<Game[]>([]); // Previous games
    const [selectedGameBoards, setSelectedGameBoards] = useState<Board[]>([]); // Boards for the selected previous game
    const [selectedGame, setSelectedGame] = useState<Game | null>(null); // Selected previous game
    const [error, setError] = useAtom(errorAtom); // Error state

    useEffect(() => {
        // Fetch boards for the current game
        const fetchActiveGameBoards = async () => {
            try {
                const token = user?.token;
                if (!token) {
                    setError('Token is not available.');
                    return;
                }

                const response = await axios.get<Board[]>(
                    `http://localhost:6329/api/Board/${activeGameId}/BoardsByGameId`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );

                // Set boards for the current game
                setBoards(response.data);
            } catch (err) {
                console.error(err);
                setError('Failed to fetch boards for the current game.');
            }
        };

        // Fetch previous games
        const fetchPreviousGames = async () => {
            try {
                const token = user?.token;
                if (!token) {
                    setError('Token is not available.');
                    return;
                }

                const response = await axios.get<Game[]>(
                    `http://localhost:6329/api/Games/previous`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );

                setPreviousGames(response.data);
            } catch (err) {
                console.error(err);
                setError('Failed to fetch previous games.');
            }
        };

        fetchActiveGameBoards();
        fetchPreviousGames();
    }, [activeGameId, user, setBoards, setError]);

    const handlePreviousGameClick = async (game: Game) => {
        try {
            const token = user?.token;
            if (!token) {
                setError('Token is not available.');
                return;
            }

            // Fetch boards for the selected previous game and logged-in player
            const response = await axios.get<Board[]>(
                `http://localhost:6329/api/Board/${game.id}/BoardsByGameId`,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            // Set boards for the selected previous game
            setSelectedGameBoards(response.data);
            setSelectedGame(game);
        } catch (err) {
            console.error(err);
            setError('Failed to fetch boards for the selected game.');
        }
    };

    const closeSelectedGame = () => {
        setSelectedGame(null);
        setSelectedGameBoards([]);
    };

    return (
        <div className={styles.container}>
            <h1>Board History</h1>
            {error && <p className={styles.errorText}>{error}</p>}

            {/* Active Game Boards */}
            <div className={styles.section}>
                <h2>Current Game Boards</h2>
                {boards.length > 0 ? (
                    <div className={styles.boardList}>
                        {boards.map((board) => (
                            <div key={board.id} className={styles.boardItem}>
                                <p><strong>Numbers:</strong> {board.numbers}</p>
                                <p><strong>Winning:</strong> {board.isWinning ? 'Yes' : 'No'}</p>
                            </div>
                        ))}
                    </div>
                ) : (
                    <p>No boards found for the current game.</p>
                )}
            </div>

            {/* Previous Games */}
            <div className={styles.section}>
                <h2>Previous Games</h2>
                {previousGames.map((game) => (
                    <div
                        key={game.id}
                        className={styles.gameItem}
                        onClick={() => handlePreviousGameClick(game)}
                    >
                        <p><strong>Start Date:</strong> {new Date(game.startDate).toLocaleDateString()}</p>
                    </div>
                ))}
            </div>

            {/* Modal for Selected Game Boards */}
            {selectedGame && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h2>Your Boards</h2>
                        <button onClick={closeSelectedGame}>&times;</button>
                        {selectedGameBoards.map((board) => (
                            <div key={board.id} className={styles.boardItem}>
                                <p><strong>Numbers:</strong> {board.numbers}</p>
                                <p><strong>Winning:</strong> {board.isWinning ? 'Yes' : 'No'}</p>
                            </div>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};

export default BoardsHistoryPage;
