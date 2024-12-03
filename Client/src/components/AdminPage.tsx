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
    const [selectedWinningNumbers, setSelectedWinningNumbers] = useState<number[]>([]);
    const [users, setUsers] = useState<User[]>([]);
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isCreateUserModalOpen, setIsCreateUserModalOpen] = useState(false);
    const [isEditUserModalOpen, setIsEditUserModalOpen] = useState(false);
    const [editUser, setEditUser] = useState<User | null>(null);

    const [newUser, setNewUser] = useState({
        userName: '',
        fullName: '',
        email: '',
        phoneNumber: '',
        password: '',
        role: ''
    });

    // Lock body scroll when any modal is open
    useEffect(() => {
        if (isModalOpen || isEditUserModalOpen || isCreateUserModalOpen) {
            document.body.style.overflow = 'hidden'; // Prevent scrolling
        } else {
            document.body.style.overflow = ''; // Restore scrolling
        }
    }, [isModalOpen, isEditUserModalOpen, isCreateUserModalOpen]);

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

    // Handle edit user click
    const handleEditUserClick = (user: User) => {
        setEditUser(user);
        setIsEditUserModalOpen(true);
        setIsModalOpen(false); // Close the view user modal
    };

    // Close modal
    const handleCloseModal = () => {
        setIsModalOpen(false);
        setIsEditUserModalOpen(false);
        setIsCreateUserModalOpen(false);
        setSelectedUser(null);
        setEditUser(null);
    };

    // Handle edit user input change
    const handleEditInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (editUser) {
            setEditUser({
                ...editUser,
                [e.target.name]: e.target.type === "number" ? parseFloat(e.target.value) : e.target.value,
            });
        }
    };

    // Handle new user input change
    const handleCreateInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setNewUser(prev => ({
            ...prev,
            [name]: value,
        }));
    };

    // Submit edited user form
    const handleEditUserSubmit = async () => {
        if (editUser) {
            try {
                const response = await fetch(`http://localhost:5229/api/player/${editUser.id}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(editUser),
                });

                if (response.ok) {
                    alert('User updated successfully');
                    // Fetch the updated list of users immediately after a successful update
                    const usersResponse = await fetch('http://localhost:5229/api/player');
                    const data = await usersResponse.json();
                    setUsers(data); // Update the local users state with fresh data from the server
                    handleCloseModal(); // Close the modal
                } else {
                    alert('Failed to update user');
                }
            } catch (error) {
                console.error('Error updating user:', error);
                alert('Error updating user');
            }
        }
    };

    // Submit new user form
    const handleCreateUserSubmit = async () => {
        if (!newUser.userName || !newUser.fullName || !newUser.email || !newUser.role || !newUser.password) {
            alert('Please fill in all fields.');
            return;
        }

        try {
            const response = await fetch('http://localhost:5229/api/Account/register', {
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
                setIsCreateUserModalOpen(false); // Close the modal
                // Clear form fields after user creation
                setNewUser({
                    userName: '',
                    fullName: '',
                    email: '',
                    phoneNumber: '',
                    password: '',
                    role: ''
                });
                // Refresh the user list
                const usersResponse = await fetch('http://localhost:5229/api/player');
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

    return (
        <div className={styles.container}>
            <h1 className={styles.header}>Admin Page</h1>

            {/* Winning Numbers Section */}
            <section className={styles.WiningSection}>
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
            </section>

            {/* Users Section */}
            <section className={styles.UserSection}>
                <h2 className={styles.subheader}>Users</h2>
                <button
                    className={styles.actionButton}
                    onClick={() => setIsCreateUserModalOpen(true)}
                >
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

            {/* Modal for User Details */}
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
                        <p><strong>Balance:</strong> {selectedUser.balance.toFixed(2)} DKK</p>
                        <p><strong>Annual Fee Paid:</strong> {selectedUser.annualFeePaid ? 'Yes' : 'No'}</p>
                        <p><strong>Created At:</strong> {new Date(selectedUser.createdAt).toLocaleDateString()}</p>
                        <button
                            className={styles.editButton}
                            onClick={() => handleEditUserClick(selectedUser)}
                        >
                            Edit User
                        </button>
                    </div>
                </div>
            )}

            {/* Modal for Edit User */}
            {isEditUserModalOpen && editUser && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            &times;
                        </button>
                        <h3>Edit User:</h3>
                        <input
                            type="text"
                            name="userName"
                            value={editUser.userName}
                            onChange={handleEditInputChange}
                            placeholder="Username"
                        />
                        <input
                            type="text"
                            name="fullName"
                            value={editUser.fullName}
                            onChange={handleEditInputChange}
                            placeholder="Full Name"
                        />
                        <input
                            type="email"
                            name="email"
                            value={editUser.email}
                            onChange={handleEditInputChange}
                            placeholder="Email"
                        />
                        <input
                            type="text"
                            name="phoneNumber"
                            value={editUser.phoneNumber}
                            onChange={handleEditInputChange}
                            placeholder="Phone Number"
                        />
                        <input
                            type="number"
                            name="balance"
                            value={editUser.balance}
                            onChange={handleEditInputChange}
                            placeholder="Balance"
                        />
                        <button
                            className={styles.submitButton}
                            onClick={handleEditUserSubmit}
                        >
                            Save Changes
                        </button>
                    </div>
                </div>
            )}

            {/* Modal for Creating New User */}
            {isCreateUserModalOpen && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            &times;
                        </button>
                        <h3>Create New User:</h3>
                        <input
                            type="text"
                            name="userName"
                            value={newUser.userName}
                            onChange={handleCreateInputChange}
                            placeholder="Username"
                        />
                        <input
                            type="text"
                            name="fullName"
                            value={newUser.fullName}
                            onChange={handleCreateInputChange}
                            placeholder="Full Name"
                        />
                        <input
                            type="email"
                            name="email"
                            value={newUser.email}
                            onChange={handleCreateInputChange}
                            placeholder="Email"
                        />
                        <input
                            type="text"
                            name="phoneNumber"
                            value={newUser.phoneNumber}
                            onChange={handleCreateInputChange}
                            placeholder="Phone Number"
                        />
                        <input
                            type="password"
                            name="password"
                            value={newUser.password}
                            onChange={handleCreateInputChange}
                            placeholder="Password"
                        />
                        <select
                            name="role"
                            value={newUser.role}
                            onChange={handleCreateInputChange}
                        >
                            <option value="">Select Role</option>
                            <option value="user">User</option>
                            <option value="admin">Admin</option>
                        </select>
                        <button
                            className={styles.submitButton}
                            onClick={handleCreateUserSubmit}
                        >
                            Create User
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default AdminPage;
