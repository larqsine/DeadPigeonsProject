import React, { useEffect } from 'react';
import { useAtom } from 'jotai';
import axios from 'axios';
import styles from './AllUserBoardsPage.module.css';
import {
    authAtom,
    errorAtom,
    gameHistoryAtom,
    playersSummaryAtom,
    selectedGameAtom,
    selectedPlayerAtom,
    boardHistoryAtom,
} from './PagesJotaiStore';

const AllUserBoardsPage: React.FC = () => {
    const [auth] = useAtom(authAtom);
    const [error, setError] = useAtom(errorAtom);
    const [gameHistory, setGameHistory] = useAtom(gameHistoryAtom);
    const [playersSummary, setPlayersSummary] = useAtom(playersSummaryAtom);
    const [selectedGame, setSelectedGame] = useAtom(selectedGameAtom);
    const [selectedPlayer, setSelectedPlayer] = useAtom(selectedPlayerAtom);
    const [boardHistory, setBoardHistory] = useAtom(boardHistoryAtom);

    interface Game {
        id: string;
        startDate: string;
        endDate: string | null;
        isClosed: boolean;
        winningNumbers?: number[] | null;
    }
    
    // Fetch all games on mount
    useEffect(() => {
        const fetchGames = async () => {
            try {
                const token = auth?.token;
                if (!token) {
                    setError('Token is not available.');
                    return;
                }

                const response = await axios.get('https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Games', {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (response.data?.data && Array.isArray(response.data.data)) {
                    setGameHistory(response.data.data);
                } else {
                    setError('Unexpected data format received.');
                }
                
            } catch (err) {
                console.error('Failed to fetch games:', err);
                setError('Failed to fetch games.');
            }
        };
        

        fetchGames();
    }, [auth, setGameHistory, setError]);

    // Handle game selection
    const handleGameClick = async (game: Game) => {
        setSelectedGame(game);

        try {
            const token = auth?.token;
            if (!token) {
                setError('Token is not available.');
                return;
            }

            const response = await axios.get(
                `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Board/${game.id}/players-summary`,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            setPlayersSummary(response.data);
        } catch (err) {
            console.error('Failed to fetch players for game:', err);
            setError('Failed to fetch players for the selected game.');
        }
    };

    // Handle player selection
    const handlePlayerClick = async (playerId: string) => {
        const selected = playersSummary.find((player) => player.playerId === playerId);
        if (!selected) {
            setError('Player not found.');
            return;
        }

        setSelectedPlayer(selected);

        try {
            const token = auth?.token;
            if (!token) {
                setError('Token is not available.');
                return;
            }

            const response = await axios.get(
                `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Board/${playerId}/BoardsByPlayerId`,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            setBoardHistory(response.data);
        } catch (err) {
            console.error('Failed to fetch boards for player:', err);
            setError('Failed to fetch boards for the selected player.');
        }
    };

    return (
        <div className={styles.container}>
            <h1>All User Boards</h1>
            {error && <p className={styles.errorText}>{error}</p>}

            {/* Games List */}
            <section className={styles.GameSection}>
                <h2>Games</h2>
                <ul className={styles.gameList}>
                    {gameHistory.length > 0 ? (
                        gameHistory.map((game) => (
                            <li
                                key={game.id}
                                className={styles.gameItem}
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
                                    <strong>Closed:</strong> {game.isClosed ? 'Yes' : 'No'}
                                </p>
                            </li>
                        ))
                    ) : (
                        <p>No games found.</p>
                    )}
                </ul>
            </section>

            {/* Players in Selected Game */}
            {selectedGame && (
                <section className={styles.PlayerSection}>
                    <h2>Players in Game</h2>
                    <ul className={styles.playerList}>
                        {playersSummary.map((player) => (
                            <li
                                key={player.playerId}
                                className={styles.playerItem}
                                onClick={() => handlePlayerClick(player.playerId)}
                            >
                                <p>
                                    <strong>Name:</strong> {player.playerName}
                                </p>
                                <p>
                                    <strong>Total Boards:</strong> {player.totalBoards}
                                </p>
                            </li>
                        ))}
                    </ul>
                </section>
            )}

            {/* Boards for Selected Player */}
            {selectedPlayer && (
                <section className={styles.BoardSection}>
                    <h2>Boards for {selectedPlayer.playerName}</h2>
                    {boardHistory.length > 0 ? (
                        <ul className={styles.boardList}>
                            {boardHistory.map((board) => (
                                <li key={board.id} className={styles.boardItem}>
                                    <p>
                                        <strong>Numbers:</strong> {board.numbers}
                                    </p>
                                    <p>
                                        <strong>Created At:</strong> {new Date(board.createdAt).toLocaleString()}
                                    </p>
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <p>No boards found for this user.</p>
                    )}
                </section>
            )}
        </div>
    );
};

export default AllUserBoardsPage;
