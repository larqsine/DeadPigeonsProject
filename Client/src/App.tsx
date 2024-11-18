import React from 'react';
import Navbar from './components/Navbar';
import styles from  './App.module.css';

const App: React.FC = () => {
    return (
        <div className={styles.app}>
                <Navbar />
        </div>
    );
};

export default App;
