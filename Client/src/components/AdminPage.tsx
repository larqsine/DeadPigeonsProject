import React, { useState, useEffect } from 'react';
import styles from './AdminPage.module.css';

type User = {
    id: string;
    userName: string;
    fullName: string;
    email: string;
    phoneNumber: string;
    balance: number;
    annualFeePaid: boolean;
    createdAt: string;
};

const AdminPage: React.FC = () => {
    // State for selected winning numbers
    const [selectedWinningNumbers, setSelectedWinningNumbers] = useState<number[]>([]);
    const [users, setUsers] = useState<User[]>([]);
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    // Fetch all users
    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetch('http://localhost:5229/api/player'); // API endpoint for fetching users
                const data = await response.json();
                setUsers(data);
            } catch (error) {
                console.error('Failed to fetch users:', error);
            }
        };

        fetchUsers();
    }, []);

    // Handle box click for winning numbers
    const handleBoxClick = (num: number) => {
        if (selectedWinningNumbers.includes(num)) {
            setSelectedWinningNumbers(selectedWinningNumbers.filter((box) => box !== num));
        } else if (selectedWinningNumbers.length < 3) {
            setSelectedWinningNumbers([...selectedWinningNumbers, num]);
        } else {
            alert('You can select a maximum of 3 winning numbers.');
        }
    };

    // Handle submit for winning numbers
    const handleSubmit = () => {
        alert(`Winning numbers are: ${selectedWinningNumbers.join(', ')}`);
    };

    // Open modal with user details
    const handleUserClick = (user: User) => {
        setSelectedUser(user);
        setIsModalOpen(true);
    };

    // Close modal
    const handleCloseModal = () => {
        setIsModalOpen(false);
        setSelectedUser(null);
    };

    return (
        <div className={styles.container}>
            <h1 className={styles.header}>Admin Page</h1>

            {/* Winning Numbers Section */}
            <section>
                <p className={styles.subheader}>Select up to 3 winning numbers:</p>
                <div className={styles.gridContainer}>
                    {Array.from({ length: 16 }, (_, i) => (
                        <div
                            key={i + 1}
                            className={`${styles.box} ${
                                selectedWinningNumbers.includes(i + 1) ? styles.selected : ''
                            }`}
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
            <section>
                <h2 className={styles.subheader}>Users</h2>
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

            {/* Modal */}
            {isModalOpen && selectedUser && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            &times;
                        </button>
                        <h3>User Details:</h3>
                        <p><strong>Username:</strong> {selectedUser.userName}</p>
                        <p><strong>Full Name:</strong> {selectedUser.fullName}</p>
                        <p><strong>Email:</strong> {selectedUser.email}</p>
                        <p><strong>Phone:</strong> {selectedUser.phoneNumber}</p>
                        <p><strong>Balance:</strong> ${selectedUser.balance.toFixed(2)}</p>
                        <p><strong>Annual Fee Paid:</strong> {selectedUser.annualFeePaid ? 'Yes' : 'No'}</p>
                        <p><strong>Created At:</strong> {new Date(selectedUser.createdAt).toLocaleDateString()}</p>
                    </div>
                </div>
            )}
        </div>
    );
};

export default AdminPage;
