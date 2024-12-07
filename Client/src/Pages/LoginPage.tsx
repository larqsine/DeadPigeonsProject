import React from 'react';
import { useAtom } from 'jotai';
import axios from 'axios';
import styles from './LoginPage.module.css';
import { loginFormAtom, userAtom, isLoggedInAtom } from './PagesJotaiStore.ts';

interface LoginPageProps {
    onLogin: (email: string, roles: string[]) => void;
}

const LoginPage: React.FC<LoginPageProps> = ({ onLogin }) => {
    // Using Jotai atoms to manage form state
    const [loginForm, setLoginForm] = useAtom(loginFormAtom);
    const [, setUser] = useAtom(userAtom);
    const [, setIsLoggedIn] = useAtom(isLoggedInAtom);

    const handleLoginChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setLoginForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleLoginSubmit = async () => {
        if (loginForm.email && loginForm.password) {
            try {
                const response = await axios.post('http://localhost:6329/api/account/login', {
                    email: loginForm.email,
                    password: loginForm.password,
                });

                const { user, roles, token } = response.data;
                alert(`Login successful! Welcome ${user}`);

                // Store user data in Jotai atoms
                setUser({ userName: user, roles, token });
                setIsLoggedIn(true); // Set the logged-in status to true

                onLogin(user, roles); 
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
