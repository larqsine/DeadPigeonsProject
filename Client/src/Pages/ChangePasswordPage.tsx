import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { useAtom } from "jotai";
import { passwordChangeRequiredAtom, usernameAtom, isLoggedInAtom } from "../AppJotaiStore";
import styles from "./ChangePasswordPage.module.css";

const ChangePasswordPage: React.FC = () => {
   // const [username] = useAtom(usernameAtom);
    const [passwordChangeRequired, setPasswordChangeRequired] = useAtom(passwordChangeRequiredAtom);
    const [isLoggedIn] = useAtom(isLoggedInAtom);
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handlePasswordChange = async () => {
        if (!currentPassword || !newPassword) {
            setError("Please fill out all fields.");
            return;
        }

        setLoading(true);
        setError(null);

        try {
            // Assume token is available in context
            const token = "your_token_here";

            const response = await axios.post(
                "http://localhost:6329/api/account/change-password",
                { currentPassword, newPassword },
                { headers: { Authorization: `Bearer ${token}` } }
            );

            alert(response.data.message);

            // Clear password change flag on successful password change
            setPasswordChangeRequired(false);
            localStorage.setItem("passwordChangeRequired", "false");

            // Redirect to main page
            navigate("/");
        } catch (err) {
            console.error("Error changing password:", err);
            setError("Failed to change password. Please check your input.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className={styles.container}>
            <h1>Change Password</h1>
            <div className={styles.form}>
                <input
                    type="password"
                    placeholder="Current Password"
                    value={currentPassword}
                    onChange={(e) => setCurrentPassword(e.target.value)}
                />
                <input
                    type="password"
                    placeholder="New Password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                />
                <button onClick={handlePasswordChange} disabled={loading}>
                    {loading ? "Changing..." : "Change Password"}
                </button>
                {error && <div className={styles.error}>{error}</div>}
            </div>
        </div>
    );
};

export default ChangePasswordPage;
