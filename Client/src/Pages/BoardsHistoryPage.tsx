import React, { useEffect, useState } from 'react';
import { useAtom } from 'jotai';
import axios from 'axios';
import styles from './BoardsHistoryPage.module.css';
import {userAtom} from './PagesJotaiStore.ts';

interface Game {
    id: string;
    startDate: string;
    endDate: string | null;
    isClosed: boolean;
    totalRevenue: number;
    prizePool: number;
    rolloverAmount: number | null;
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

const BoardHistoryPage: React.FC = () => {
    const [user] = useAtom(userAtom);
    const [activeGame, setActiveGame] = useState<Game | null>(null);
    const [previousGames, setPreviousGames] = useState<Game[]>([]);
    const [activeGameBoards, setActiveGameBoards] = useState<Board[]>([]);
    const [selectedGameBoards, setSelectedGameBoards] = useState<Board[]>([]);
    const [selectedGame, setSelectedGame] = useState<Game | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchGames = async () => {
            try {
                const token = user?.token;
                if (!token) {
                    setError('Token is not available.');
                    return;
                }

                // Fetch all games
                const gamesResponse = await axios.get<{ message: string; data: Game[] }>(
                    'http://localhost:6329/api/Games',
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
                const games = gamesResponse.data.data;

                // Fetch the active game ID
                const activeGameResponse = await axios.get<{ gameId: string }>(
                    'http://localhost:6329/api/Games/active',
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
                const activeGameId = activeGameResponse.data.gameId;
                
                const active = games.find((game) => game.id === activeGameId) || null;
                const previous = games.filter((game) => game.isClosed);

                setActiveGame(active);
                setPreviousGames(previous);
                
                if (active) {
                    const activeBoardsResponse = await axios.get<Board[]>(
                        `http://localhost:6329/api/Board/${active.id}/BoardsbyGameId`,
                        {
                            headers: {
                                Authorization: `Bearer ${token}`,
                            },
                        }
                    );
                    setActiveGameBoards(activeBoardsResponse.data);
                }
            } catch (err) {
                console.error(err);
                setError('Failed to fetch games and boards.');
            }
        };

        fetchGames();
    }, [user]);

    const handleGameClick = async (game: Game) => {
        try {
            const token = user?.token;
            if (!token) {
                setError('Token is not available.');
                return;
            }

            // Fetch boards for the selected game
            const boardsResponse = await axios.get<Board[]>(
                `http://localhost:6329/api/Board/${game.id}/BoardsbyGameId`,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            // Filter boards to only include those for the current player
            const playerBoards = boardsResponse.data.filter((board) => board.isWinning || true);
            setSelectedGameBoards(playerBoards);
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

            {/* Active Game Section */}
            <div className={styles.section}>
                <h2>Active Game</h2>
                {activeGame ? (
                    <>
                        <p>
                            <strong>Start Date:</strong> {new Date(activeGame.startDate).toLocaleDateString()}
                        </p>
                        <div className={styles.boardList}>
                            {activeGameBoards.map((board) => (
                                <div key={board.id} className={styles.boardItem}>
                                    <p>
                                        <strong>Numbers:</strong> {board.numbers}
                                    </p>
                                    <p>
                                        <strong>Fields Count:</strong> {board.fieldsCount}
                                    </p>
                                    <p>
                                        <strong>Cost:</strong> {board.cost} DKK
                                    </p>
                                    <p>
                                        <strong>Created At:</strong> {new Date(board.createdAt).toLocaleString()}
                                    </p>
                                    <p>
                                        <strong>Winning:</strong> {board.isWinning ? 'Yes' : 'No'}
                                    </p>
                                </div>
                            ))}
                        </div>
                    </>
                ) : (
                    <p className={styles.message}>No active game found.</p>
                )}
            </div>

            {/* Previous Games Section */}
            <div className={styles.section}>
                <h2>Previous Games</h2>
                {previousGames.length === 0 ? (
                    <p className={styles.message}>No previous games found.</p>
                ) : (
                    previousGames.map((game) => (
                        <div
                            key={game.id}
                            className={styles.gameSection}
                            onClick={() => handleGameClick(game)}
                        >
                            <p>
                                <strong>Start Date:</strong> {new Date(game.startDate).toLocaleDateString()}
                            </p>
                            <p>
                                <strong>End Date:</strong>{' '}
                                {game.endDate ? new Date(game.endDate).toLocaleDateString() : 'Ongoing'}
                            </p>
                            <p>
                                <strong>Winning Numbers:</strong>{' '}
                                {game.winningNumbers ? game.winningNumbers.join(', ') : 'Not available'}
                            </p>
                        </div>
                    ))
                )}
            </div>

            {/* Selected Game Boards Section */}
            {selectedGame && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h2>Your Boards for this game:</h2>
                        <button onClick={closeSelectedGame} className={styles.closeButton}>
                            &times;
                        </button>
                        <div className={styles.boardList}>
                            {selectedGameBoards.map((board) => (
                                <div key={board.id} className={styles.boardItem}>
                                    <p>
                                        <strong>Numbers:</strong> {board.numbers}
                                    </p>
                                    <p>
                                        <strong>Fields Count:</strong> {board.fieldsCount}
                                    </p>
                                    <p>
                                        <strong>Cost:</strong> {board.cost} DKK
                                    </p>
                                    <p>
                                        <strong>Created At:</strong> {new Date(board.createdAt).toLocaleString()}
                                    </p>
                                    <p>
                                        <strong>Winning:</strong> {board.isWinning ? 'Yes' : 'No'}
                                    </p>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            )}

            {error && <p className={styles.errorText}>{error}</p>}
        </div>
    );
};

export default BoardHistoryPage;
