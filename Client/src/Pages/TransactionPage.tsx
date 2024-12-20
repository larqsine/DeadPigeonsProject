import React, { useEffect } from "react";
import { useAtom } from "jotai";
import {transactionsAtom, loadingAtom, errorAtom, authAtom} from "./PagesJotaiStore";
import styles from "./TransactionPage.module.css";

export interface Transaction {
    id: string;
    amount: number;
    status: number;
    transactionType: string;
    createdAt?: string;
}

const TransactionPage: React.FC = () => {
    const [transactions, setTransactions] = useAtom(transactionsAtom);
    const [loading, setLoading] = useAtom(loadingAtom);
    const [error, setError] = useAtom(errorAtom);
    const [auth] = useAtom(authAtom);


    useEffect(() => {
        const fetchTransactions = async () => {
            setLoading(true);
            setError("");
            try {
                const response = await fetch("https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/Transaction/deposit", {});
                if (!response.ok) {
                    const errorText = await response.text();
                    console.error("Error response:", errorText);
                    throw new Error(`Failed to fetch transactions: ${response.statusText}`);
                }
                const data: Transaction[] = await response.json();
                const filteredTransactions = data.filter(transaction => transaction.status === 0);
                setTransactions(filteredTransactions);
            } catch (err: unknown) {
                if (err instanceof Error) {
                    console.error("Error fetching transactions:", err.message);
                    setError(err.message);
                }
            } finally {
                setLoading(false);
            }
        };

        fetchTransactions();
    }, [setTransactions, setLoading, setError]);


    const handleApprove = async (transactionId: string) => {
        try {
            const token = auth || localStorage.getItem('token');

            const response = await fetch(
                `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/transaction/${transactionId}/approve`,
                {
                    method: "PUT",
                    headers: {
                        "Authorization": `Bearer ${token}`,
                    },
                }
            );

            if (!response.ok) {
                throw new Error("Failed to approve transaction.");
            }
            alert("Transaction approved successfully!");
            setTransactions((prev) => prev.filter((t) => t.id !== transactionId));
        } catch (err: unknown) {
            if (err instanceof Error) {
                alert(err.message);
            }
        }
    };

    const handleDecline = async (transactionId: string) => {
        try {
            const token = auth || localStorage.getItem('token');

            const response = await fetch(
                `https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/transaction/${transactionId}/decline`,
                {
                    method: "PUT",
                    headers: {
                        "Authorization": `Bearer ${token}`,
                    },
                }
            );
            if (!response.ok) {
                throw new Error("Failed to decline transaction.");
            }
            alert("Transaction declined successfully!");
            setTransactions((prev) => prev.filter((t) => t.id !== transactionId));
        } catch (err: unknown) {
            if (err instanceof Error) {
                alert(err.message);
            }
        }
    };
    if (loading) {
        return <div className={styles.container}>Loading transactions...</div>;
    }
    if (error) {
        return <div className={styles.container} style={{ color: "red" }}>{error}</div>;
    }

    return (
        <div className={styles.container}>
            <h1 className={styles.title}>Transaction Management</h1>
            {transactions.length === 0 ? (
                <p>No deposit transactions found.</p>
            ) : (
                <ul className={styles.transactionList}>
                    {transactions.map((transaction) => (
                        <li key={transaction.id} className={styles.transactionItem}>
                            <div className={styles.transactionDetails}>
                                <p><strong>ID:</strong> {transaction.id}</p>
                                <p><strong>Amount:</strong> {transaction.amount}</p>
                                <p><strong>Status:</strong> {transaction.status}</p>
                            </div>
                            <div className={styles.transactionActions}>
                                <button className={styles.approve} onClick={() => handleApprove(transaction.id)}>
                                    Approve
                                </button>
                                <button className={styles.decline} onClick={() => handleDecline(transaction.id)}>
                                    Decline
                                </button>
                            </div>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default TransactionPage;
