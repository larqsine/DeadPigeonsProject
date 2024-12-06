import { atom } from 'jotai';

export const isLoggedInAtom = atom(false);
export const isAdminAtom = atom(false);
export const showBoxGridAtom = atom(true);
export const transitioningAtom = atom(false);
export const usernameAtom = atom<string | null>(null);
export const balanceAtom = atom<number | null>(null);
