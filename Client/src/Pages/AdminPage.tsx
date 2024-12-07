import React, { useEffect } from 'react';
import { useAtom } from 'jotai';
import styles from './AdminPage.module.css';
import {
    selectedWinningNumbersAtom,
    usersAtom,
    selectedUserAtom,
    isModalOpenAtom,
    isCreateUserModalOpenAtom,
    isEditUserModalOpenAtom,
    editUserAtom,
    newUserAtom, User, gameIdAtom, errorAtom, messageAtom,
} from './PagesJotaiStore.ts';
import axios from "axios";


const AdminPage: React.FC = () => {
    const [selectedWinningNumbers, setSelectedWinningNumbers] = useAtom(selectedWinningNumbersAtom);
    const [users, setUsers] = useAtom(usersAtom);
    const [selectedUser, setSelectedUser] = useAtom(selectedUserAtom);
    const [isModalOpen, setIsModalOpen] = useAtom(isModalOpenAtom);
    const [isCreateUserModalOpen, setIsCreateUserModalOpen] = useAtom(isCreateUserModalOpenAtom);
    const [isEditUserModalOpen, setIsEditUserModalOpen] = useAtom(isEditUserModalOpenAtom);
    const [editUser, setEditUser] = useAtom(editUserAtom);
    const [newUser, setNewUser] = useAtom(newUserAtom);
    const [ gameId,setGameId] = useAtom(gameIdAtom);
    const [, setError] = useAtom(errorAtom);
    const [, setMessage] = useAtom(messageAtom);
    

    // Lock body scroll when any modal is open
    useEffect(() => {
        if (isModalOpen || isEditUserModalOpen || isCreateUserModalOpen) {
            document.body.classList.add('modal-open');
        } else {
            document.body.classList.remove('modal-open');
        }
    }, [isModalOpen, isEditUserModalOpen, isCreateUserModalOpen]);

    // Fetch all users
    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetch('http://localhost:6329/api/player');
                if (!response.ok) throw new Error('Failed to fetch users');
                const data = await response.json();
                setUsers(data);
            } catch (error) {
                console.error('Failed to fetch users:', error);
            }
        };

        const fetchGameData = async () => {
            try {
                const response = await axios.get(`http://localhost:6329/api/Games/active`);
                setGameId(response.data.gameId);
            } catch (err) {
                setError('Failed to fetch game data');
            }
        };
        
        fetchGameData();
        fetchUsers();
    }, [setUsers, setGameId, setError]);

    const handleBoxClick = (num: number) => {
        if (selectedWinningNumbers.includes(num)) {
            setSelectedWinningNumbers(selectedWinningNumbers.filter((box) => box !== num));
        } else if (selectedWinningNumbers.length < 3) {
            setSelectedWinningNumbers([...selectedWinningNumbers, num]);
        } else {
            alert('You can select a maximum of 3 winning numbers.');
        }
    };

    const handleSubmit = async () => {
        const payload = {
            winningNumbers: selectedWinningNumbers, 
            gameId: gameId,
        };
        
        console.log('GameId:', gameId); // Logs the active game ID
        console.log('Number', selectedWinningNumbers); //Logs the selected wining numbers
        console.log('Payload:', payload); // Logs the payload sent to the backend

        try {
            const response = await axios.post(`http://localhost:6329/api/Games/${gameId}/close`, payload);
            setMessage(response.data.message || 'Game closed successfully!');
            setSelectedWinningNumbers([]); // Clear selected numbers after purchase
        } catch (err) {
            if (axios.isAxiosError(err)) {
                console.error('Error Status:', err.response?.status);
                console.error('Error Response:', err.response?.data);
                setError(err.response?.data?.message || 'An error occurred during closing the game.');
            } else {
                console.error('Unexpected Error:', err);
                setError('An unexpected error occurred.');
            }
        }
        alert(`Winning numbers are: ${selectedWinningNumbers.join(', ')}`);
    };
    
    const handeNewGame = async () => {
        try {
            const response = await axios.post(`http://localhost:6329/api/Games/new`);
            setMessage(response.data.message || 'Game created successfully!');
        }
        catch (err){
            if (axios.isAxiosError(err)) {
                console.error('Error Status:', err.response?.status);
                console.error('Error Response:', err.response?.data);
                setError(err.response?.data?.message || 'An error occurred during creating the game.');
            } else {
                console.error('Unexpected Error:', err);
                setError('An unexpected error occurred.');
            }
        }
    };

    const handleUserClick = (user: User) => {
        setSelectedUser(user);
        setIsModalOpen(true);
    };

    const handleEditUserClick = (user: User) => {
        setEditUser(user);
        setIsEditUserModalOpen(true);
        setIsModalOpen(false);
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setIsEditUserModalOpen(false);
        setIsCreateUserModalOpen(false);
        setSelectedUser(null);
        setEditUser(null);
    };

    const handleEditInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (editUser) {
            setEditUser({
                ...editUser,
                [e.target.name]: e.target.type === 'number' ? parseFloat(e.target.value) : e.target.value,
            });
        }
    };

    const handleCreateInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setNewUser((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleEditUserSubmit = async () => {
        if (editUser) {
            try {
                const response = await fetch(`http://localhost:6329/api/player/${editUser.id}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(editUser),
                });

                if (response.ok) {
                    alert('User updated successfully');
                    const usersResponse = await fetch('http://localhost:6329/api/player');
                    const data = await usersResponse.json();
                    setUsers(data);
                    handleCloseModal();
                } else {
                    alert('Failed to update user');
                }
            } catch (error) {
                console.error('Error updating user:', error);
                alert('Error updating user');
            }
        }
    };

    const handleCreateUserSubmit = async () => {
        if (!newUser.userName || !newUser.fullName || !newUser.email || !newUser.role || !newUser.password) {
            alert('Please fill in all fields.');
            return;
        }

        try {
            const response = await fetch('http://localhost:6329/api/Account/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    userName: newUser.userName,
                    fullName: newUser.fullName,
                    email: newUser.email,
                    phone: newUser.phoneNumber,
                    password: newUser.password,
                    role: newUser.role,
                }),
            });

            const result = await response.json();

            if (response.ok) {
                alert('User created successfully');
                setIsCreateUserModalOpen(false);
                setNewUser({
                    userName: '',
                    fullName: '',
                    email: '',
                    phoneNumber: '',
                    password: '',
                    role: '',
                });
                const usersResponse = await fetch('http://localhost:6329/api/player');
                const data = await usersResponse.json();
                setUsers(data);
            } else {
                alert(result.message || 'Failed to create user');
            }
        } catch (error) {
            console.error('Error creating user:', error);
            alert('An error occurred while creating the user.');
        }
    };

    const handleDeleteUser = async (userId: string) => {
        const confirmed = window.confirm('Are you sure you want to delete this user?');
        if (!confirmed) return;

        try {
            const response = await fetch(`http://localhost:6329/api/player/${userId}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                alert('User deleted successfully');
                setUsers(users.filter((user) => user.id !== userId));
                handleCloseModal();
            } else {
                alert('Failed to delete user');
            }
        } catch (error) {
            console.error('Error deleting user:', error);
            alert('Error deleting user');
        }
    };

    return (
        <div className={styles.container}>
            <h1 className={styles.header}>Admin Page</h1>

            {/* Winning Numbers Section */}
            <section className={styles.WiningSection}>
                <button
                    className={styles.actionButton}
                    onClick={handeNewGame}
                >
                    Start New Game
                </button>
                <p className={styles.subheader}>Select up to 3 winning numbers:</p>
                <div className={styles.gridContainer}>
                    {Array.from({length: 16}, (_, i) => (
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

            </section>

            {/* Users Section */}
            <section className={styles.UserSection}>
                <h2 className={styles.subheader}>Users</h2>
                <button className={styles.actionButton} onClick={() => setIsCreateUserModalOpen(true)}>
                New User
                </button>
                <ul className={styles.userList}>
                    {users.map((user) => (
                        <li
                            key={user.id}
                            className={styles.userItem}
                            onClick={() => handleUserClick(user)}
                        >
                            {user.userName}
                        </li>
                    ))}
                </ul>
            </section>

            {isModalOpen && selectedUser && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            &times;
                        </button>
                        <h3>User Details</h3>
                        <p>UserName: {selectedUser.userName}</p>
                        <p>Full Name: {selectedUser.fullName}</p>
                        <p>Email: {selectedUser.email}</p>
                        <p>Phone Number: {selectedUser.phoneNumber}</p>
                        <p>Balance: {selectedUser.balance}</p>
                        <p>Annual Fee Paid: {selectedUser.annualFeePaid ? 'Yes' : 'No'}</p>
                        <p>Created At: {selectedUser.createdAt}</p>
                        <div className={styles.modalButtons}>
                            <button onClick={() => handleEditUserClick(selectedUser)}>Edit User</button>
                            <button onClick={() => handleDeleteUser(selectedUser.id)}>Delete User</button>
                        </div>
                    </div>
                </div>
            )}


            {/* Modal for Editing User */}
            {isEditUserModalOpen && editUser && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Edit User</h3>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            &times;
                        </button>
                        <form onSubmit={(e) => {
                            e.preventDefault();
                            handleEditUserSubmit();
                        }}>
                            <label>
                                User Name:
                                <input
                                    type="text"
                                    name="userName"
                                    value={editUser.userName}
                                    onChange={handleEditInputChange}
                                />
                            </label>
                            <label>
                                Full Name:
                                <input
                                    type="text"
                                    name="fullName"
                                    value={editUser.fullName}
                                    onChange={handleEditInputChange}
                                />
                            </label>
                            <label>
                                Email:
                                <input
                                    type="email"
                                    name="email"
                                    value={editUser.email}
                                    onChange={handleEditInputChange}
                                />
                            </label>
                            <label>
                                Phone Number:
                                <input
                                    type="text"
                                    name="phoneNumber"
                                    value={editUser.phoneNumber}
                                    onChange={handleEditInputChange}
                                />
                            </label>
                            <label>
                                Annual Fee Paid:
                                <input
                                    type="checkbox"
                                    name="annualFeePaid"
                                    checked={editUser.annualFeePaid}
                                    onChange={() => setEditUser({
                                        ...editUser,
                                        annualFeePaid: !editUser.annualFeePaid
                                    })}
                                />
                            </label>
                            <button type="submit">Save Changes</button>
                        </form>
                    </div>
                </div>
            )}

            {/* Modal for Creating User */}
            {isCreateUserModalOpen && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Create New User</h3>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            &times;
                        </button>
                        <form onSubmit={(e) => {
                            e.preventDefault();
                            handleCreateUserSubmit();
                        }}>
                            <label>
                                User Name:
                                <input
                                    type="text"
                                    name="userName"
                                    value={newUser.userName}
                                    onChange={handleCreateInputChange}
                                />
                            </label>
                            <label>
                                Full Name:
                                <input
                                    type="text"
                                    name="fullName"
                                    value={newUser.fullName}
                                    onChange={handleCreateInputChange}
                                />
                            </label>
                            <label>
                                Email:
                                <input
                                    type="email"
                                    name="email"
                                    value={newUser.email}
                                    onChange={handleCreateInputChange}
                                />
                            </label>
                            <label>
                                Phone Number:
                                <input
                                    type="text"
                                    name="phoneNumber"
                                    value={newUser.phoneNumber}
                                    onChange={handleCreateInputChange}
                                />
                            </label>
                            <label>
                                Password:
                                <input
                                    type="password"
                                    name="password"
                                    value={newUser.password}
                                    onChange={handleCreateInputChange}
                                />
                            </label>
                            <label>
                                Role:
                                <select
                                    name="role"
                                    value={newUser.role}
                                    onChange={handleCreateInputChange}
                                >
                                    <option value="admin">Admin</option>
                                    <option value="player">Player</option>
                                    {/* Updated option */}
                                </select>
                            </label>
                            <button type="submit">Create User</button>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

export default AdminPage;
    