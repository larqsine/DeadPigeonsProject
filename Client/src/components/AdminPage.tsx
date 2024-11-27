import React, { useState, useEffect } from 'react';
import styles from './AdminPage.module.css';

// Type for player data
type Player = {
    id: string;
    name: string;
    email: string;
    role: string;
    balance: number;
    createdAt: string;
};

const AdminPage: React.FC = () => {
    // State for selected winning numbers
    const [selectedWinningNumbers, setSelectedWinningNumbers] = useState<number[]>([]);

    // State for managing players
    const [players, setPlayers] = useState<Player[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Fetch all players when component mounts
    useEffect(() => {
        fetchPlayers();
    }, []);

    const fetchPlayers = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await fetch('/api/player'); // Fetch players from backend
            const data = await response.json();
            setPlayers(data);
        } catch (error) {
            setError('Failed to fetch players.');
        } finally {
            setLoading(false);
        }
    };

    const handleDeletePlayer = async (playerId: string) => {
        if (window.confirm('Are you sure you want to delete this player?')) {
            try {
                await fetch(`/api/player/${playerId}`, { method: 'DELETE' }); // Call DELETE endpoint
                setPlayers(players.filter(player => player.id !== playerId)); // Optimistic UI update
            } catch (error) {
                alert('Failed to delete player.');
            }
        }
    };

    const handleEditPlayer = async (playerId: string) => {
        // You can either show an inline form or a modal for editing player details
        const player = players.find(p => p.id === playerId);
        if (player) {
            const newName = prompt('Edit player name:', player.name);
            if (newName) {
                try {
                    const updatedPlayerResponse = await fetch(`/api/player/${player.id}`, {
                        method: 'PUT',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ ...player, name: newName })
                    });
                    const updatedPlayer = await updatedPlayerResponse.json();
                    setPlayers(players.map(p => (p.id === player.id ? updatedPlayer : p)));
                } catch (error) {
                    alert('Failed to update player.');
                }
            }
        }
    };

    const handleAddPlayer = async (newPlayer: Player) => {
        try {
            const response = await fetch('/api/player', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newPlayer)
            });
            const addedPlayer = await response.json();
            setPlayers([...players, addedPlayer]);
        } catch (error) {
            alert('Failed to add player.');
        }
    };

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

            {/* Winning Numbers Section */}
            <p className={styles.subheader}>Select up to 3 winning numbers:</p>
            <div className={styles.gridContainer}>
                {Array.from({ length: 16 }, (_, i) => (
                    <div
                        key={i + 1}
                        className={`${styles.box} ${selectedWinningNumbers.includes(i + 1) ? styles.selected : ''}`}
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

            {/* Player Management Section */}
            <h2 className={styles.subheader}>Manage Players</h2>

            <button className={styles.actionButton} onClick={() => handleAddPlayer({ /* player details */ })}>
                Add New Player
            </button>

            {loading && <p>Loading players...</p>}
            {error && <p className={styles.error}>{error}</p>}

            <table className={styles.table}>
                <thead>
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Role</th>
                    <th>Balance</th>
                    <th>Created At</th>
                    <th>Actions</th>
                </tr>
                </thead>
                <tbody>
                {players.map(player => (
                    <tr key={player.id}>
                        <td>{player.id}</td>
                        <td>{player.name}</td>
                        <td>{player.email}</td>
                        <td>{player.role}</td>
                        <td>{player.balance}</td>
                        <td>{player.createdAt}</td>
                        <td>
                            <button onClick={() => handleEditPlayer(player.id)}>Edit</button>
                            <button onClick={() => handleDeletePlayer(player.id)}>Delete</button>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default AdminPage;
