import React, { useState } from 'react';
import axios from 'axios';
import styles from './LoginPage.module.css';

interface LoginPageProps {
    onLogin: (email: string, roles: string[]) => void;
}

const LoginPage: React.FC<LoginPageProps> = ({ onLogin }) => {
    const [loginForm, setLoginForm] = useState({ email: '', password: '' });

    const handleLoginChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setLoginForm((prev) => ({ ...prev, [name]: value }));
    };
    
    const handleLoginSubmit = async () => {
        if (loginForm.email && loginForm.password) {
            try {
                const response = await axios.post('http://localhost:5229/api/account/login', {
                    email: loginForm.email,
                    password: loginForm.password,
                });


                const { user, roles } = response.data;
                alert(`Login successful! Welcome ${user}`);
                onLogin(user, roles); // Pass user details to parent component or state
            } catch (error) {
                console.log(error);
                console.error(error);
                alert('Login failed. Please check your credentials.');
            }
        } else {
            alert('Please enter email and password.');
        }
    };
    

    return (
        <div className={styles.container}>
            <img src="/logo.png" alt="App Logo" className={styles.logo} />
            <div className={styles.content}>
                <h1>{'Login'}</h1>
                    <div className={styles.form}>
                        <input
                            type="text"
                            name="email"
                            placeholder="Email"
                            value={loginForm.email}
                            onChange={handleLoginChange}
                        />
                        <input
                            type="password"
                            name="password"
                            placeholder="Password"
                            value={loginForm.password}
                            onChange={handleLoginChange}
                        />
                        <button onClick={handleLoginSubmit}>Login</button>
                    </div>
            </div>
        </div>
    );
};

export default LoginPage;