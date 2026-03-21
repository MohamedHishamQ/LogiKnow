'use client';

import { useState, useEffect } from 'react';
import { useTranslations } from 'next-intl';
import { useAuth } from '@/hooks/useAuth';
import { apiClient, AcademicEntryDto } from '@/api/client';
import ProtectedRoute from '@/components/ProtectedRoute';
import { Loader2, ShieldCheck, CheckCircle2, XCircle, AlertCircle } from 'lucide-react';

interface PendingSubmission extends AcademicEntryDto {
  submitterEmail?: string;
  submitterName?: string;
}

export default function AdminDashboard() {
  const { user } = useAuth();
  const t = useTranslations('Common');
  const [submissions, setSubmissions] = useState<PendingSubmission[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [error, setError] = useState('');
  const [rejectReason, setRejectReason] = useState<{id: string, reason: string} | null>(null);

  const isAdminOrModerator = user?.roles?.includes('Admin') || user?.roles?.includes('Moderator');

  useEffect(() => {
    if (isAdminOrModerator) {
      loadPendingSubmissions();
    } else {
      setLoading(false);
    }
  }, [isAdminOrModerator]);

  const loadPendingSubmissions = async () => {
    try {
      const { data } = await apiClient.get('/moderation/pending');
      setSubmissions(data.data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load submissions');
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async (id: string) => {
    setActionLoading(id);
    try {
      await apiClient.post(`/moderation/${id}/approve`);
      setSubmissions(prev => prev.filter(s => s.id !== id));
    } catch (err: any) {
      setError(err.message || 'Failed to approve');
    } finally {
      setActionLoading(null);
    }
  };

  const handleReject = async (id: string) => {
    if (!rejectReason || rejectReason.id !== id || !rejectReason.reason.trim()) {
      alert('Please provide a reason for rejection.');
      return;
    }
    
    setActionLoading(id);
    try {
      await apiClient.post(`/moderation/${id}/reject`, { reason: rejectReason.reason });
      setSubmissions(prev => prev.filter(s => s.id !== id));
      setRejectReason(null);
    } catch (err: any) {
      setError(err.message || 'Failed to reject');
    } finally {
      setActionLoading(null);
    }
  };

  if (!isAdminOrModerator && !loading) {
    return (
      <ProtectedRoute>
        <div className="min-h-screen py-20 px-4 text-center">
          <ShieldCheck className="w-16 h-16 text-red-400 mx-auto mb-4" />
          <h1 className="text-2xl font-bold text-white mb-2">Access Denied</h1>
          <p className="text-white/60">You need Admin or Moderator privileges to view this page.</p>
        </div>
      </ProtectedRoute>
    );
  }

  return (
    <ProtectedRoute>
      <div className="min-h-screen manar-page-bg p-8">
        <div className="max-w-7xl mx-auto space-y-8">
          <header className="mb-10">
            <h1 className="text-3xl md:text-4xl font-black mb-2 text-white flex items-center gap-3 tracking-tight">
              <ShieldCheck className="w-8 h-8 text-manar-cyan" />
              Admin Dashboard
            </h1>
            <p className="text-blue-200/70">
              Manage platform content, review submissions, and oversee users.
            </p>
          </header>

          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
            {/* Stats Cards */}
            {[
              { label: 'Pending Reviews', value: submissions.length, color: 'border-manar-gold/50 text-manar-gold' },
              { label: 'Total Terms', value: '0', color: 'border-white/10 text-white' },
              { label: 'Total Books', value: '0', color: 'border-white/10 text-white' },
              { label: 'Total Academic', value: '0', color: 'border-white/10 text-white' }
            ].map((stat, i) => (
              <div key={i} className={`glass-card p-6 rounded-2xl border ${stat.color} flex flex-col justify-center`}>
                <span className="text-white/60 text-sm font-medium mb-1">{stat.label}</span>
                <span className={`text-4xl font-black ${stat.color.replace('border-', 'text-').split('/')[0]}`}>
                  {loading ? '-' : stat.value}
                </span>
              </div>
            ))}
          </div>

          <div className="glass-card rounded-3xl p-8">
            <h2 className="text-xl font-bold text-white mb-6 flex items-center gap-2">
              <AlertCircle className="w-5 h-5 text-manar-cyan" />
              Pending Submissions
            </h2>

            {error && (
              <div className="bg-red-500/10 text-red-400 p-4 rounded-xl mb-6 border border-red-500/20">
                {error}
              </div>
            )}

            {loading ? (
              <div className="flex justify-center py-12">
                <Loader2 className="w-8 h-8 text-manar-cyan animate-spin" />
              </div>
            ) : submissions.length === 0 ? (
              <div className="text-center py-16 px-4 border border-dashed border-white/20 rounded-2xl bg-white/5">
                <CheckCircle2 className="w-12 h-12 text-emerald-400/50 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-white mb-1">All caught up!</h3>
                <p className="text-white/60">There are no pending submissions to review.</p>
              </div>
            ) : (
              <div className="space-y-6">
                {submissions.map((sub) => (
                  <div key={sub.id} className="bg-white/5 border border-white/10 rounded-2xl p-6 transition-all hover:bg-white/10">
                    <div className="flex flex-col md:flex-row justify-between gap-6">
                      <div className="space-y-3 flex-1">
                        <div className="flex items-center gap-3">
                          <span className="px-2.5 py-1 rounded-full text-xs font-bold bg-manar-cyan/20 text-manar-cyan border border-manar-cyan/30">
                            {sub.type}
                          </span>
                          <span className="text-white/40 text-sm">{new Date().toLocaleDateString()}</span>
                        </div>
                        <h3 className="text-xl font-bold text-white">{sub.title}</h3>
                        <p className="text-white/70 text-sm leading-relaxed line-clamp-2">{sub.abstract}</p>
                        
                        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 pt-3 border-t border-white/10 text-sm">
                          <div>
                            <span className="block text-white/40 text-xs mb-1">Author</span>
                            <span className="text-white">{sub.author}</span>
                          </div>
                          <div>
                            <span className="block text-white/40 text-xs mb-1">Institution</span>
                            <span className="text-white">{sub.university}</span>
                          </div>
                          <div>
                            <span className="block text-white/40 text-xs mb-1">Year</span>
                            <span className="text-white">{sub.year}</span>
                          </div>
                          {sub.documentUrl && (
                            <div>
                              <span className="block text-white/40 text-xs mb-1">Document</span>
                              <a href={sub.documentUrl} target="_blank" rel="noreferrer" className="text-manar-cyan hover:underline">View PDF</a>
                            </div>
                          )}
                        </div>
                      </div>

                      <div className="flex flex-col justify-start gap-3 min-w-[200px]">
                        {rejectReason?.id === sub.id ? (
                          <div className="space-y-3 animate-in fade-in slide-in-from-right-4">
                            <textarea
                              placeholder="Reason for rejection..."
                              className="w-full bg-black/40 border border-white/20 rounded-xl p-3 text-sm text-white focus:ring-1 focus:ring-red-400 focus:border-red-400 outline-none resize-none"
                              rows={3}
                              value={rejectReason.reason}
                              onChange={(e) => setRejectReason({ id: sub.id, reason: e.target.value })}
                            />
                            <div className="flex gap-2">
                              <button
                                onClick={() => handleReject(sub.id)}
                                disabled={actionLoading === sub.id || !rejectReason.reason.trim()}
                                className="flex-1 bg-red-500 hover:bg-red-600 text-white py-2 rounded-lg text-sm font-bold transition-colors disabled:opacity-50"
                              >
                                {actionLoading === sub.id ? <Loader2 className="w-4 h-4 animate-spin mx-auto" /> : 'Confirm'}
                              </button>
                              <button
                                onClick={() => setRejectReason(null)}
                                className="px-3 bg-white/10 hover:bg-white/20 text-white rounded-lg text-sm transition-colors"
                              >
                                Cancel
                              </button>
                            </div>
                          </div>
                        ) : (
                          <>
                            <button
                              onClick={() => handleApprove(sub.id)}
                              disabled={actionLoading === sub.id}
                              className="w-full bg-emerald-500/20 hover:bg-emerald-500/30 text-emerald-400 border border-emerald-500/50 py-2.5 rounded-xl font-bold flex items-center justify-center gap-2 transition-all disabled:opacity-50"
                            >
                              {actionLoading === sub.id ? <Loader2 className="w-5 h-5 animate-spin" /> : <CheckCircle2 className="w-5 h-5" />}
                              Approve
                            </button>
                            <button
                              onClick={() => setRejectReason({ id: sub.id, reason: '' })}
                              disabled={actionLoading === sub.id}
                              className="w-full bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/30 py-2.5 rounded-xl font-bold flex items-center justify-center gap-2 transition-all disabled:opacity-50"
                            >
                              <XCircle className="w-5 h-5" />
                              Reject
                            </button>
                          </>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
