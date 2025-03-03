﻿import React, { useEffect, useState } from 'react';
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
    totalRevenue: number;
    prizePool: number;
    rolloverAmount?: number | null;
    winningNumbers?: number[] | null;
}

interface ApiResponse<T> {
    message: string;
    data: T;
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
    const [user] = useAtom(userAtom);
    const [boards, setBoards] = useAtom(boardHistoryAtom);
    const [activeGameId] = useAtom(gameIdAtom);
    const [previousGames, setPreviousGames] = useState<Game[]>([]);
    const [selectedGameBoards, setSelectedGameBoards] = useState<Board[]>([]);
    const [selectedGame, setSelectedGame] = useState<Game | null>(null);
    const [error, setError] = useAtom(errorAtom);

    useEffect(() => {
        const fetchActiveGameBoards = async () => {
            try {
                const token = user?.token;

                const response = await axios.get<Board[]>(
                    `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Board/${activeGameId}/BoardsByGameId`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
                
                setBoards(response.data);
            } catch (err) {
                console.error(err);
                setError('Failed to fetch boards for the current game.');
            }
        };

        const fetchPreviousGames = async () => {
            try {
                const token = user?.token;

                const response = await axios.get<ApiResponse<Game[]>>(
                    `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Games/closed`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
                
                const sortedGames = response.data.data.sort(
                    (a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
                );
                setPreviousGames(sortedGames);
            } catch (err) {
                console.error(err);
                setError('Failed to fetch closed games.');
            }
        };

        fetchActiveGameBoards();
        fetchPreviousGames();
    }, [activeGameId, user, setBoards, setError]);

    const handlePreviousGameClick = async (game: Game) => {
        try {
            const token = user?.token;
            
            const response = await axios.get<Board[]>(
                `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Board/${game.id}/BoardsByGameId`,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

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
                        <p><strong>End
                            Date:</strong> {game.endDate ? new Date(game.endDate).toLocaleDateString() : 'Ongoing'}</p>
                        <p><strong>Prize Pool:</strong> {game.prizePool.toFixed(2)} DKK</p>
                        <p><strong>Total Revenue:</strong> {game.totalRevenue.toFixed(2)} DKK</p>
                        {game.winningNumbers && (
                            <p><strong>Winning Numbers:</strong> {game.winningNumbers.join(', ')}</p>
                        )}
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
